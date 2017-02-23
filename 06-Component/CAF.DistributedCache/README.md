##分支说明##

该分支是基于BSF的基础上开发的分支。<br/>
修改内容<br/>
1）sdk以插件的形式扩展自BSF。<br/>
2）项目命名空间从Dyd.Base.DistributedCache修改为DistributedCache<br/>
3) 打包安装包，可以直接被第三方安装使用。<br/>
4）若使用旧版本XXF.dll，请则使用master分支<br/>
。
##分布式缓存中间件##
  方便实现缓存的分布式，集群，负载均衡，故障自动转移，并兼容多种缓存存储的分布式缓存中间件。 用于解决分布式架构中的分布式缓存环节。

##特点：##
 1. 代码少，便于扩展。
 2. 兼容阿里云memcache，redis，ssdb。
 3. 规范缓存使用接口，屏蔽底层缓存实现。
 4. 通过配置连接字符串即可切换不同存储引擎，可以混合不同存储引擎组成缓存集群部署。（如部分redis，部分memcache）
 5. 动态负载均衡，故障转移，线上无缝平行扩展和扩容，方便运维。

##不同存储介质##
       

        /// <summary>
        /// Redis 
        /// 数据存内存,适合内存大小范围内大量缓存。（若是频繁失效的缓存数据，大量热点数据，建议使用redis）
        /// </summary>
        Redis,
        /// <summary>
        /// SSDB
        /// 数据热点存内存，大量数据存磁盘。（若是命中率较低，命中热点数据，大量冷数据，建议使用ssdb）
        /// </summary>
        SSDB,
        /// <summary>
        /// Memcached
        /// </summary>
        Memcached,
        /// <summary>
        /// SQLServer内存表
        /// </summary>
        SqlServer,
        /// <summary>
        /// 阿里云的缓存服务OCS
        /// </summary>
        AliyunMemcached,

##备注：##
 1. 属于半研究性项目，已在线上阿里云memcache环境使用。

##未来发展:##
 1. 分布式缓存中间件平台化，实现缓存监控，预警，性能报告等，性能数据收集至监控平台。
 2. 扩展分布式缓存的其他特点。
 3. 环形一致性hash对负载均衡和故障转移的支持。  

开源相关群: .net 开源基础服务 **238543768**<br/>
(大家都有本职工作，也许不能及时响应和跟踪解决问题，请谅解。)

##.net 开源第三方开发学习路线 ##

- 路线1:下载开源源码->学习开源项目->成功部署项目（根据开源文档或者QQ群项目管理员协助）->成为QQ群相关项目管理员->了解并解决日常开源项目问题->总结并整理开源项目文档并分享给大家或推广->成为git项目的开发者和参与者
- 路线2:下载开源源码->学习开源项目->成功部署项目（根据开源文档或者QQ群项目管理员协助）->在实际使用中发现bug并提交bug给项目相关管理员
- 路线3:下载开源源码->学习开源项目->成功部署项目（根据开源文档或者QQ群项目管理员协助）->自行建立开源项目分支->提交分支新功能给项目官方开发人员->官方开发人员根据项目情况合并新功能并发布新版本

## 关于.net 开源生态圈的构想 ##
<b>.net 生态闭环</b>：官方开源项目->第三方参与学习->第三方改进并提交新功能或bug->官方合并新功能或bug->官方发布新版本<br/>
<b>为什么开源?</b> .net 开源生态本身弱,而强大是你与我不断学习，点滴分享,相互协助，共同营造良好的.net生态环境。<br/>
<b>开源理念:</b> 开源是一种态度，分享是一种精神，学习仍需坚持，进步仍需努力，.net生态圈因你我更加美好。<br/>

by 车江毅