using System.Threading.Tasks;

namespace DemoWebMvc.Helper
{
    public interface IServiceHelper
    {
        /// <summary>
        /// 获取产品数据
        /// </summary>
        /// <returns></returns>
        Task<string> GetProductAsync(string accessToken);

        /// <summary>
        /// 获取订单数据
        /// </summary>
        /// <returns></returns>
        Task<string> GetOrderAsync(string accessToken);

        /// <summary>
        /// 获取服务列表
        /// </summary>
        void GetServices();
    }
}
