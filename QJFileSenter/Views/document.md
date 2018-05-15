# REST Finder API
*********************************


## 上传文件API
+ GET http://127.0.0.1:8080/document/fileupload 上传示例界面
+ POST http://127.0.0.1:8080/document/fileupload

## 获取文件API
+ GET http://127.0.0.1:8080/document/{md5}
注：md5为上传后返回的md5主键，不存在的md5不返回文件。

## 获取文件信息API
+ GET http://127.0.0.1:8080/document/info/{md5}
返回举例：
<pre><code>
{
  "md5": "7D-8A-61-09-CA-D3-AC-02-46-17-80-B1-07-A9-16-FD", 
  "name": "安全监测系统图", 
  "extension": "dwg", 
  "mimetype": "application/octet-stream", 
  "rdate": "2015/7/16 10:14:50"
}
</code></pre>