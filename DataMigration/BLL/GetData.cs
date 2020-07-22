using DataMigration.DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataMigration.BLL
{
    public class GetData
    {
        #region 数据表查询语句
        public const string EXPORT_SELECT_ALL_SQLROW = @"SELECT a.name AS TABLE_NAME,b.rows AS ROWS FROM sysobjects a
                                             INNER JOIN sysindexes b ON a.id=b.id  
                                             WHERE b.indid IN(0,1) AND a.Type='u' AND a.name IN ";
        public const string EXPORT_SELECT_TABLE_TYPE = @"SELECT TABLE_NAME,COLUMN_NAME from information_schema.columns WHERE DATA_TYPE in ('bit','int')";
        public const string EXPORT_SELECT_ALL_TABLES = @"SELECT DISTINCT ST.name AS 'tablename', 'INSERT INTO `' + ST.name + '` (' + SCC.tablename + ') VALUES ' AS 'insertscript' FROM sys.tables ST
                                                                                    INNER JOIN
                                                                                    (
                                                                                    	SELECT 
                                                                                    		SUBSTRING
                                                                                    		((
                                                                                    			SELECT ', `'+ CONVERT(VARCHAR(500), SCS.name) + '`'
                                                                                    			from sys.columns SCS 
                                                                                    			where SCS.object_id = SC.object_id	
                                                                                    			FOR XML PATH('')		
                                                                                    		) ,3,1000) AS tablename,SC.object_id AS obj_id
                                                                                    	FROM  sys.columns SC
                                                                                    )SCC ON SCC.obj_id = ST.object_id
                                                                                    WHERE type = 'U' ORDER BY ST.name";

        public const string EXPORT_SELECT_ALL_TABLES_FIELDS = @"SELECT DISTINCT ST.name as 'tablename', SCC.tablename  as 'insertscript' FROM sys.tables ST
                                                                                                    INNER JOIN
                                                                                                    (
                                                                                                        SELECT
                                                                                                            SUBSTRING
                                                                                                            ((
                                                                                                                SELECT ', ['+ CONVERT(VARCHAR(500), scS.name) +']'
                                                                                                                from sys.columns SCS
                                                                                                                where SCS.object_id = SC.object_id  
                                                                                                                FOR XML PATH('')      
                                                                                                            ) ,3,1000) AS tablename, SC.object_id AS obj_id
                                                                                                        FROM  sys.columns SC
                                                                                                    )
                                                                                                    SCC    ON SCC.obj_id = ST.object_id
                                                                                                    WHERE type = 'U' ORDER BY ST.name";

        public const string EXPORT_SELECT_ALL_TABLES_MYSQL = @"SELECT IT.TABLE_NAME FROM information_schema.TABLES IT WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = ";
        public const string EXPORT_SELECT_ALL_TABLES_MYSQL_FIELDS = @"SELECT TABLE_NAME,COLUMN_NAME FROM information_schema.Columns WHERE TABLE_SCHEMA = ";
        public const string EXPORT_SELECT_ALL_TABLES_SQL_SUM = @"IF object_id(N'tempdb.dbo.#temp', N'U') IS NOT NULL 
																			DROP TABLE [dbo].#temp 
																			CREATE TABLE #temp(
																				FIELD VARCHAR (100),
																				RESULT bigint
																			)
																			DECLARE @SQL nvarchar (MAX)
																			DECLARE infoCursor CURSOR FOR SELECT
																				'INSERT INTO #temp(FIELD,RESULT) SELECT ''' + TABLE_NAME + '''+''@''+''' + COLUMN_NAME + ''',SUM(Cast(' + COLUMN_NAME + ' AS ' + DATA_TYPE + ')) FROM ' + TABLE_NAME
																			FROM
																				(
																					SELECT
																						TABLE_NAME,
																						COLUMN_NAME,
																						(
																							CASE DATA_TYPE
																							WHEN 'int' THEN
																								'bigint'
																							ELSE
																								DATA_TYPE
																							END
																						) DATA_TYPE
																					FROM
																						information_schema.columns
																				) a
																			WHERE
																				DATA_TYPE IN (
																					'bigint',
																					'int',
																					'smallint'
																				)
																			AND COLUMN_NAME NOT IN ('ID')
																			AND TABLE_NAME IN {0}
																			{1}
																			ORDER BY
																				TABLE_NAME,
																				COLUMN_NAME OPEN infoCursor FETCH NEXT
																			FROM
																				infoCursor INTO @SQL
																			WHILE @@FETCH_STATUS = 0
																			BEGIN
																				EXEC (@SQL) PRINT @SQL FETCH NEXT
																			FROM
																				infoCursor INTO @SQL
																			END CLOSE infoCursor DEALLOCATE infoCursor 
																			SELECT FIELD, RESULT FROM #temp";
        public const string EXPORT_SELECT_ALL_TABLES_MYSQL_SQLS = @"DROP TABLE IF EXISTS temp;

                                                                CREATE TABLE temp (sqls VARCHAR(500));

                                                                INSERT INTO temp (sqls) SELECT
                                                                	CONCAT(
                                                                		'insert into temp(FIELD,RESULT) select  CONCAT(''',
                                                                		TABLE_NAME,
                                                                		''',''@'',''',
                                                                		COLUMN_NAME,
                                                                		'''), sum(`',
                                                                		COLUMN_NAME,
                                                                		'`)  from ',
                                                                		TABLE_NAME,
                                                                		';'
                                                                	)
                                                                FROM
                                                                	information_schema. COLUMNS,
                                                                	(SELECT @rownum := 0) AS it
                                                                WHERE
                                                                COLUMN_NAME NOT IN ('ID')
                                                                AND DATA_TYPE IN ('bigint', 'int', 'smallint', 'tinyint')
                                                                AND TABLE_NAME IN {0}
                                                                {1}
                                                                AND TABLE_SCHEMA = '{2}'
                                                                ORDER BY
                                                                	TABLE_NAME,
                                                                	COLUMN_NAME;
                                                                    
                                                                SELECT sqls FROM temp;";
        public const string EXPORT_SELECT_ALL_TABLES_MYSQL_PROCEDURE = @"delimiter //
                                                                drop procedure if exists Store_TableRows;  
                                                                CREATE PROCEDURE Store_TableRows()  
                                                                BEGIN
                                                                
                                                                declare CreateSql varchar(500);
                                                                declare done int;  
                                                                declare cur_test CURSOR for SELECT table_name FROM information_schema.tables WHERE table_schema = DATABASE() AND table_type = 'BASE TABLE';
                                                                DECLARE CONTINUE HANDLER FOR NOT FOUND SET done=1;
                                                                
                                                                PREPARE a FROM 'DROP TABLE IF EXISTS cut_table_rows';
                                                                EXECUTE a;
                                                                DEALLOCATE PREPARE a;
                                                                
                                                                PREPARE b FROM 'CREATE TABLE cut_table_rows(table_name varchar(500) DEFAULT NULL,rows int(11) DEFAULT NULL) ENGINE=InnoDB DEFAULT CHARSET=utf8';
                                                                EXECUTE b;
                                                                DEALLOCATE PREPARE b;
                                                                
                                                                open cur_test;
                                                                  posLoop:LOOP
                                                                    FETCH  cur_test into  CreateSql;
                                                                        IF done=1 THEN LEAVE posLoop;END IF;
                                                                    
                                                                    SELECT CONCAT('insert into cut_table_rows(table_name,rows)SELECT \'',table_name,'\' table_name,COUNT(*) rows FROM ',table_name) into @sql
                                                                		 FROM information_schema.tables WHERE table_schema = DATABASE() AND table_type = 'BASE TABLE' and table_name=CreateSql;
                                                                	
                                                                	PREPARE s FROM  @sql;
                                                                	EXECUTE s;
                                                                	DEALLOCATE PREPARE s;
                                                                	
                                                                  END LOOP posLoop;
                                                                CLOSE cur_test;
                                                                
                                                                PREPARE C FROM 'SELECT * FROM cut_table_rows';
                                                                EXECUTE C;
                                                                DEALLOCATE PREPARE C;
                                                                
                                                                END;";
        #endregion
        /// <summary>
        /// Sql取数据
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static SqlDataReader GetSqlDataReader(string connStr, string comm)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(connStr);
                SqlCommand command = sqlConn.CreateCommand();
                command.CommandTimeout = 999999;
                sqlConn.Open();
                command.CommandType = CommandType.Text;
                command.CommandText = comm;
                SqlDataReader rdr = command.ExecuteReader();
                return rdr;
            }
            catch (Exception e)
            {
                WirterLogs.WriteFile(e);
                MessageBox.Show(e.Message);
                return null;
            }
        }
        /// <summary>
        /// MySql取数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static MySqlDataReader GetMySqlDataReader(string connStr, string comm)
        {
            MySqlConnection mySqlconn = new MySqlConnection(connStr + "Connection Timeout=300");
            try
            {
                MySqlCommand cmd = mySqlconn.CreateCommand();
                cmd.CommandTimeout = 999999;
                mySqlconn.Open();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = comm;
                MySqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception e)
            {
                WirterLogs.WriteFile(e);
                MessageBox.Show(e.Message);
                return null;
            }
        }
        /// <summary>
        /// 执行Mysql语句
        /// </summary>
        public static bool ExecuTransMySQL(string connstr, string sql)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connstr))
                {
                    conn.Open();
                    MySqlScript script = new MySqlScript(conn);
                    script.Query = sql;
                    script.Execute();
                }
            }
            catch (Exception e)
            {
                WirterLogs.WriteFile(e);
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 执行插入语句
        /// </summary>
        public static void GetMysqlDBConnection_Execute(string connStr, Dictionary<string, List<StringBuilder>> queryToExecute, ProgressBar progressBar, RichTextBox ListLog)
        {
            MySqlConnection conn = new MySqlConnection(connStr + "Connection Timeout=180");
            conn.Open();
            string errorMsg = string.Empty;
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandTimeout = 999999;
            List<StringBuilder> item;
            try
            {
                if (queryToExecute != null && queryToExecute.Count > 0)
                {

                    foreach (string table in queryToExecute.Keys)
                    {
                        errorMsg = table;
                        queryToExecute.TryGetValue(table, out item);
                        foreach (StringBuilder comm in item)
                        {
                            MySqlTransaction transaction = conn.BeginTransaction();
                            try
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Add("", MySqlDbType.Blob);
                                string commStr = comm.ToString();
                                cmd.CommandText = commStr.Substring(0, commStr.Length - 3);
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                progressBar.Invoke((Action)(() => progressBar.Value = 0));
                                ListLog.Invoke((Action)(() => ListLog.SelectionColor = Color.Red));
                                ListLog.Invoke((Action)(() => ListLog.AppendText(table + "执行中止:" + e.Message + Environment.NewLine)));
                                ListLog.Invoke((Action)(() => ListLog.Focus()));
                            }
                        }
                    }
                }
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                progressBar.Invoke((Action)(() => progressBar.Value = 0));
                ListLog.Invoke((Action)(() => ListLog.SelectionColor = Color.Red));
                ListLog.Invoke((Action)(() => ListLog.AppendText("执行中止:" + ex.Message + Environment.NewLine)));
                ListLog.Invoke((Action)(() => ListLog.Focus()));
                WirterLogs.WriteFile(ex);
                MessageBox.Show("语句插入出现错误:" + errorMsg + Environment.NewLine + ex.Message);
            }
        }
        /// <summary>
        /// 执行插入语句
        /// </summary>
        public static void DBConnection_Execute(string connStr, Dictionary<string, List<StringBuilder>> queryToExecute)
        {
            MySqlConnection conn = new MySqlConnection(connStr + "Connection Timeout=180");
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandTimeout = 999999;
            List<StringBuilder> item;
            foreach (string table in queryToExecute.Keys)
            {
                queryToExecute.TryGetValue(table, out item);
                foreach (StringBuilder comm in item)
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("", MySqlDbType.Blob);
                    string commStr = comm.ToString();
                    cmd.CommandText = commStr.Substring(0, commStr.Length - 3);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
