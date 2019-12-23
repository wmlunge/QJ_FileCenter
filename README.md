## Lotus云盘
##### Lotus云盘的前身是专门为外部系统提供文件上传，下载，在线浏览等服务的一个组件，最新的2.0版本在这个组件基础上集成了原有协同平台的云盘功能，目前已经变成一个可以独立部署并且可以为其它系统提供文件管理服务的系统，比较适合面向个人、团队或小型组织来搭建属于自己的网盘。


### 更新内容
### 2019-12-24 版本说明(V2.2)，主要是添加了视频得转码播放及音频播放功能
1. 集成了ffmpeg.exe组件，在后台进行视频转码，保证上传视频能够在线播放
2. 添加了音乐播放功能，支持MP3格式文件播放
3. 修复密码重置有问题得Bug
4. 修复文件共享无法看见得问题
5. 新增/document/viedo/{zyid}接口，用来实现分片传输视频文件，否则无法实现拖动播放
6. 完善若干细节
##### 说明：ffmpeg.exe转码对机器性能要求很高...转码得时间大约等同于视频时间，机器越好转的越快，qt-faststart.exe用来实现边下边播功能，不然就只能等视频下载完才能播放，

### 2019-10-07 版本说明(V2.1)，主要是添加了第三方系统的支持以及移动端页面
1. 更换了更加强大的图片浏览插件以及视频查看的插件，功能更加完善及美观
2. 添加了第三方系统集成的机制，第三方网站可以通过Iframe的方式集成Lotus云盘，集成方式见[官网教程](http://www.qijiekeji.com)
3. 添加移动端页面，系统用户端支持在手机端访问，手机访问自适应移动端

### 2019-09-18 版本说明(V2.0)，主要是在原有的功能上集成了简单的云盘功能
1. 后台管理端添加了用户管理功能，用户分为两组，管理员（可以进入管理端），普通用户（只能查查看企业空间，个人空间，用户共享三个模块）
2. 增加了系统配置功能，可以在后台配置端口,IP以及文件存放路径功能
3. 集成了WebUpload上传组件（拖拽上传,文件夹上传,断点续传，秒传，粘贴等等）
3. 完善了文件夹及文件的分享功能，支持外部分享，和内部分享
4. 支持文件的压缩与解压功能,支持打包下载，支持预览zip包内容功能
5. 支持WORD，PPT,PDF格式的在线预览功能
6. 支持视频在线播放

### 后期升级计划
- 支持手机端web功能
- 支持更多文件的预览功能，比如txt，音频文件,视频文件之类的
- 更全面的后台管理功能
- 文件的备份，移动
- 文件服务器的操作日志
- 多帐号管理，帐号安全控制
- 更丰富的API
- 全局搜索功能（最好也能搜索文件内容）
- 提升组件的性能
### 演示环境
[点击此处进入演示环境](http://114.67.237.16:9100/Web/Login.html)
演示账号:yhy,密码:123456

[点击此处下载安装包](http://114.67.237.16:9100/Web/Html/Tools/share.html?ID=12)

### 版权说明
- 本框架遵循GPL开源协议,企业单位如商用请联系作者授权
- 有问题请尽量在群里或者码云上提问，QQ讨论群：538014542
- 二次开发后的系统只允许内部使用，不得进行出租、出售。

### 系统主要截图
![主页](https://images.gitee.com/uploads/images/2019/0919/161658_83335e13_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0919/161752_59784343_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0919/161818_d0ff5fc4_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0919/161837_defb482a_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0919/230925_28826e49_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0919/230957_ebdb804e_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0922/162040_8e8e1b41_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0922/162347_04f1b526_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0922/162211_b60b63a5_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0922/162232_6a13ce7f_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/0922/162304_7dfb1705_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/1223/130404_3f9f7c63_11702.png "屏幕截图.png")
![输入图片说明](https://images.gitee.com/uploads/images/2019/1223/130252_5ac5163b_11702.png "屏幕截图.png")

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
已经打包成exe文件，装完即用,非常方便
#### 二：使用方便：使用方式包含以下2种：

- 单独部署，作为个人或小型企业网盘使用
     
- 作为服务组件供其它系统使用
    上传方式非常简单，系统目前支持两种方式上传,一种是集成了百度的WebUpload插件，通过一段JS即可调用上传组件上传,就是下面这个玩意,【同时也支持普通的post上传文件】。
#### 三：功能强大：
1.WebUpload多强大，看下面官网这张图就明白了，什么拖拽上传,文件夹上传,断点续传，秒传，粘贴，分片什么的完全不在话下，你自己搞这些，还能比它搞的更好吗？反正我是不行
![输入图片说明](https://images.gitee.com/uploads/images/2019/0919/161355_7c47a0e1_11702.png "WebUpload组件")
2.目前已经支持PDF,WORD,PPT格式的文件预览功能,后续会继续支持其它格式的文件预览功能

3.支持文件的压缩与解压功能,支持打包下载，支持预览zip包内容功能

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

