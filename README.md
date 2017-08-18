

**AssetUX项目文档**

AssetUX project documentation

# 一、设计目标

1、资源版本控制；

2、AssetBundle生成；

3、AssetBundle下载；

4、AssetBundle加载；

5、Xlua使用；

&ensp; 

一、Design goals

1、Resource version control

2、AssetBundle generated

3、AssetBundle download

4、AssetBundle load

5、Xlua use


	

# 二、资源版本控制

## 资源生成

选中大版本，生成对应版本目录，将所有assetbundle打包成一个压缩包；

选中小版本，生成对应版本目录，将assetbundle放到该目录下；

这两种方式，都生成版本控制文件；



项目中每个版本中资源差异暂时使用SVN或git做管理，后面增加功能；
&ensp; 
&ensp; 
**版本命名规范**

第一部分为主版本号，

第二部分为次版本号，

第三部分为修订版本号，

第四部分为日期版本号加希腊字母版本号，

希腊字母版本号共有五种，分别为base、alpha、beta 、RC 、 release

如：1.1.1.20170817\_RC	  


&ensp; 
&ensp; 
二、Resource version control

Resources generated

Selected large version, generate the corresponding version catalogue, all the assetbundle packaged into a package;

Selected small version, generate the corresponding version of the directory, put the assetbundle in that directory;

Both ways, generate version control file;

Project resource differences in each version temporarily use SVN or git do management, adding features behind;





## 客户端版本控制

获取服务端版本文件，解析，选择是否更新大版本或是小版本；

大版本更新；

下载资源压缩包，完成后，删除之前版本资源文件，解压；

小版本更新；

根据版本文件，下载对应资源，保存或替换原有资源；

&ensp; 
&ensp; 

The client version control

Access to the server version file, parse, choose update whether big or small versions;

Major version update;

Download package, complete, delete the previous version resource file, extract;

Small version update;

According to the version of the file, download the corresponding resources, save or replacing the original resources;







# 三、AssetBundle生成

生成assetbundle、zip等文件放在Assets上层目录下的AssetBundles目录，根据平台不同有Android、Windows、IOS、等目录、在平台目录下有版本目录；

&ensp; 
&ensp; 

AssetBundle generated

Generate assetbundle, zip files in Assets upper AssetBundles directory in the directory, depending on the platform such as Android, Windows, IOS, directory, have a version in the platform directory directory;;



# 四、AssetBundle下载

可实现断点续传；

可查看当前进度；

&ensp; &ensp; 

AssetBundle download

It can realize breakpoint continuingly;

To view the current schedule



# 五、AssetBundle加载

可以加载AssetBUndle里的资源或场景；

&ensp; &ensp; 

AssetBundle load

Can load the resources in the AssetBUndle or scene;



# 六、Xlua使用

参考Xlua项目文档。  
&ensp; &ensp; 

Xlua use

Reference Xlua project  documentation.

# 联系
qq群：629868435

# contact
QQ group：629868435


