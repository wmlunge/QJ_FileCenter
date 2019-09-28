using System;
using System.Collections;
using System.Runtime.Caching;


namespace QJ_FileCenter
{
    /// <summary>
    /// HttpRuntime Cache读取设置缓存信息封装
    /// 使用描述：给缓存赋值使用HttpRuntimeCache.Set(key,value....)
    /// 读取缓存中的值使用JObject jObject=HttpRuntimeCache.Get(key) as JObject，读取到值之后就可以进行一系列判断
    /// </summary>
    public class CacheHelp
    {

        public ObjectCache cache = MemoryCache.Default;//声明缓存类

        /// <summary>
        /// 设置缓存，没有其他重载方法，第一个参数name是我们的缓存的名字，第二个参数是我们需要缓存的对象，第三个是我们的过期时间默认7200秒
        /// </summary>
        /// <param name="name">缓存的名字</param>
        /// <param name="Ovlaue">需要缓存的值</param>
        /// <param name="seconds">过期时间</param>
        public void Set(string name, object Ovlaue, int seconds = 72000)
        {
            CacheItemPolicy policy = new CacheItemPolicy();

            policy.AbsoluteExpiration = DateTime.Now.AddSeconds(seconds);

            cache.Set(name, Ovlaue, policy);
        }

        /// <summary>
        /// 获取缓存，传入缓存名字即可
        /// </summary>
        /// <param name="name">缓存的名字</param>
        /// <returns></returns>
        public string Get(string name)
        {
            string strReturn = "";
            if (cache.Contains(name))
            {
                strReturn= cache[name].ToString();
            }
            return strReturn;
        }
     
      
    }





}
