# sqlserver-mysql
sqlserver数据迁移至mysql
将sqlserver中的数据迁移至mysql
	1.需提前在mysql中建立表结构相同的数据库
	2.可以针对不同的表加条件限制，语法规则符合sqlserver语法
	3.迁移数据表以两个数据库共有的表为准
	4.每个表数据使用各自线程进行迁移，提升速度
