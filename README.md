

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

简单方法增量更新，每个版本生成assetbundle后，放到下载目录，目录名是版本号，如1.0.2.5可作为目录名。
		
Simple method, incremental updates, each version generated after the assetbundle, into the download directory, directory name is the version number.
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


## 客户端版本控制
客户端选择下载最新版本

　　１．获取服务端最新版本号，对比本地版本;
　　
　　２．如果有新版本，根据本地保存的下一次更新的版本号下载；
　　
　　３．如果没有，直接进入游戏。


&ensp; 
&ensp; 
The client choose to download the latest version

1.The server's latest version number, comparing local version;

2.If there is a new version, according to the local save the version number of the next update to download;

3.If not, go into the game.


&ensp; 
&ensp; 

版本差异包管理的两种方式:

　　 第一种是每次出新版本时，只需要生成与之前一个版本的差异包，
　　　　玩家需要跨多个版本更新时，需要下载多个差异包。

			
　　　　优点是每次出版本只需要出一个差异包；
　　　　
　　　　缺点就是玩家如果跨很多个版本更新时，将会耗费更多的时间和流量。
　　　　
&ensp; 
&ensp; 
			
　　第二种是每次出新版本时，将最新版本和之前每一个版本都做对比，
　　
　　　　生成多个差异包，例如现在要出1.3的新版，那么开发团队需要做
　　　　得就是生成一个1.3与1.1的差异包，再生成一个1.3和1.2的差异包，
　　　　这样玩家就可以从任何一个版本一次性升级到最新版本。

　　　　优点是玩家跨版本更新的时间和流量都减少；
　　　　
　　　　缺点是每次出版本需要耗费的时间更长。



# 三、AssetBundle生成

选择AssetUX菜单可以生成assetbundle、版本文件，根据平台不同有Android、Windows、IOS、等目录、在平台目录下有版本目录。

&ensp; 
&ensp; 

AssetBundle generated

Select AssetUX menu can generate assetbundle, version file, depending on the platform such as Android, Windows, IOS, catalogue, the platform has a version of the directory directory.



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

# 感谢
感谢sNaticY / assetUpdater-core ，unity assetbundleManager，Tencent/xLua等开源项目。
&ensp; &ensp; 
Thank sNaticY/assetUpdater - core, unity assetbundleManager, Tencent/xLua open source project.

# 联系
qq群：629868435

# contact
QQ group：629868435



