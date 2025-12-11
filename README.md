## 一、前言
主要逻辑来自于 https://github.com/ldqk/Masuit.LuceneEFCore.SearchEngine ，非常感谢大佬的开源，🙏

## 二、开发工具和框架
使用VS2022开发，项目为.net8

## 三、主要说明
仍然是Lucent.NET的封装，作为单独类库

但是将创建索引和检索查询这两部分拆分开，尽量独立

(1)api项目主要保存数据到关系型数据库，然后redis将信息保存到List中

(2)定时项目从List中取出数据，并做处理同步更新到索引中

## 四、示例

参见：Example.md

## 五、项目说明

### (1)Kp.Api
为检索查询的api项目，做api使用，数据落地关系型数据库，使用redis作为队列使用

### (2)Kp.Entity

关系型数据库的实体

### (3)Kp.LuceneIndexManager
为创建索引的项目，从redis队列中读取需要写的数据

### (4)Kp.LuceneSearchEngine

搜索引擎，包含创建索引和索引查询

### (5)Kp.LuceneSearchEngine.BaseEntity

搜索引擎的实体基类

### (6)Kp.LuceneSearchEngine.Entity

搜索引擎的实体

### (7)Kp.LuceneSearchEngine.Util

搜索引擎的公共方法
