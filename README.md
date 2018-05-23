# QJ_FileCenter

#### 项目介绍
能够独立部署的文件中心服务，提供上传，下载，文件预览等服务

### 为什么会开发这玩意...
当然是为了方便，最开始的时候是打算按照常用的方式把所有上传的文件都放到UPLOAD目录下的，这样做会面临不少问题

- 文件不好管理----上传的文件就往UPLOAD目录里扔,就没有下文了,后期管理起来只能通过Windows的资源管理器来管理了,这种方式简单的系统应付起来还行，稍微复杂点就有点力不从心了

- 方式不够漂亮----文件存储和WEB程序都在一起，感觉有改善的空间

- 影响WEB效率----当下载和上传操作较多时可能会影响web执行的效率，如果能把WEB服务和文件服务分开就好了

- 不太方便扩展----或者说扩展起来比较费事，比方说做断点续传，秒传，做文件预览，等等

- 重复工作太多----每次开发一个新系统,上传这块都要全部搞一遍,感觉太费劲,以后还很难再继续升级

只要系统涉及到频繁的文件上传下载可能就都会面临这些个个问题，既然这样，为什么不把这一块单独拎出来开发成一个服务呢，于是就有了这个QJ_FileCenter组件。

### 先说优点
#### 一：部署方便: 
基本上是装完即用,非常方便,不需要学习新东西
#### 二：使用方便：
上传方式非常简单，系统目前支持两种方式上传,一种是集成了百度的WebUpload插件，通过一段JS即可调用上传组件上传,就是下面这个玩意,【同时也支持普通的post上传文件】

![输入图片说明](https://static.oschina.net/uploads/img/201805/18221428_8Ro7.png "在这里输入图片标题")
![输入图片说明](https://static.oschina.net/uploads/img/201805/18221353_pzXC.png "在这里输入图片标题")


#### 三：功能强大：
1.WebUpload多强大，看下面官网这张图就明白了，什么拖拽上传,文件夹上传,断点续传，秒传，粘贴，分片什么的完全不在话下，你自己搞这些，还能比它搞的更好吗？反正我是不行

![输入图片说明](https://static.oschina.net/uploads/img/201805/18220742_QuBF.png "WebUpload组件")

2.目前已经支持PDF,WORD,PPT格式的文件预览功能,后续会继续支持其它格式的文件预览功能

3.支持文件的压缩与解压功能,支持打包下载，支持预览zip包内容功能

4.支持后端管理功能，管理文件中心上传的文件,后续也会有更多相关的辅助功能(文件转移,文件备份什么的)

![输入图片说明](https://static.oschina.net/uploads/img/201805/18221806_Zfj2.png "管理首页")
![输入图片说明](https://static.oschina.net/uploads/img/201805/18221831_hPW4.png "文件管理")

5.目前提供了一部分基础的API,供用户调用.(例如获取压缩图片，获取office文档转化后的图片)

6.支持PC端的同时也支持移动端

7.组件提供了空间的概念，可以建立多个空间,同时为多个系统提供文件存储服务，统一管理上传的文件，免去了系统较多时文件分散在各个地方的烦恼


#### 四：提高开发效率:
开发人员不用再操心和文件相关的操作了,所有相关的操作都由QJFileCenter来处理,大大提高了开发人员的效率

### 使用技术:
- 文件信息存储在sqllite数据库里
- API框架使用Nancy
- 管理端页面使用QJ_Onelotus
- office预览采用Aspose转化成图片形式

### 后续优化计划:
- 支持更多文件的预览功能，比如txt，音频文件,视频文件之类的
- 更全面的后台管理功能
- 文件的备份，移动
- 文件服务器的操作日志
- 多帐号管理，帐号安全控制
- 更丰富的API
- 全局搜索功能（最好也能搜索文件内容）
- 提升组件的性能

### 赞助我
如果我的努力值得你的肯定，请赞助我，让我在开源的路上，做更好，走更远。
赞助我的方式包括：`支付宝打赏`、`微信打赏`、`给一个star`、`向我反馈意见和建议`

> 对我进行打赏赞助的朋友，麻烦在赞助的时候留一下你的姓名或者昵称，感谢。

### 支付宝打赏赞助
![输入图片说明](https://gitee.com/uploads/images/2018/0521/121655_17557365_11702.png "屏幕截图.png")

### 微信打赏赞助
![输入图片说明](https://gitee.com/uploads/images/2018/0521/121858_4ea39eb7_11702.png "屏幕截图.png")