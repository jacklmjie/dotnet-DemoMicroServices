using System;
using System.Threading.Tasks;
using DemoOrderAPI.MessageDto;
using DemoOrderAPI.Models;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DemoOrderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICapPublisher _capBus;
        private readonly OrderContext _context;

        public OrdersController(ILogger<OrdersController> logger, IConfiguration configuration, ICapPublisher capPublisher, OrderContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _capBus = capPublisher;
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string result = $"【订单服务】{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}——" +
                $"{Request.HttpContext.Connection.LocalIpAddress}:{_configuration["ConsulSetting:ServicePort"]}";
            return Ok(result);
        }

        /// <summary>
        /// 下单 发布下单事件
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            using (var trans = _context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                //业务代码
                order.CreateTime = DateTime.Now;
                _context.Orders.Add(order);

                var r = await _context.SaveChangesAsync() > 0;

                if (r)
                {
                    //发布下单事件
                    await _capBus.PublishAsync("order.services.createorder",
                        new CreateOrderMessageDto()
                        {
                            Count = order.Count,
                            ProductID = order.ProductID
                        });
                    return Ok();
                }
                return BadRequest();
            }
        }
    }
}
