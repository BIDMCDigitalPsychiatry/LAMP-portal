
/*************************************************************************************************

	[LAMP] SCRIPT -- CREATE DATABASE & USER

*************************************************************************************************/


/*-------------- CREATING DATABASE -------------*/

USE master
GO

BEGIN
	
	DECLARE @FilePath	NVARCHAR(1000)
	DECLARE @DBName		NVARCHAR(1000)

	SET @FilePath = 'C:\LAMP\DBFiles\'	   /** Enter the File Location **/
    SET @DBName   = 'LAMP'                /** Enter the Database Name **/
    
	IF NOT EXISTS (SELECT NAME FROM SYSDATABASES WHERE NAME=@DBName )
	BEGIN 
		EXECUTE 
		( 'CREATE DATABASE ['+ @DBName +'] ON  PRIMARY 
		  ( NAME = N'''+@DBName +''', FILENAME =  N''' + @FilePath + @DBName + '.mdf''' +', SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
		  LOG ON ' +
		 '( NAME = N'''+@DBName +'_log'', FILENAME =N''' + @FilePath + @DBName + '_log.ldf'''+' , SIZE = 768KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) ' 
		)
	END
END	
GO
 

/*-------------- CREATING USER -------------*/


USE [LAMP]	/** Enter the Database Name **/ 
GO

BEGIN
	DECLARE @DBName		NVARCHAR(50)
	DECLARE @UserName	NVARCHAR(50)
	DECLARE @Password	NVARCHAR(50)

	SET @DBName   = 'LAMP'			/**  Enter the Database Name **/
	SET @UserName = 'lampadmin'			/**  Enter the UserName **/
	SET @Password = 'password123?'		/**  Enter the Password **/

	-------------------------------------------------------------------------
	IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = @UserName)
	BEGIN
		EXECUTE('DROP LOGIN ' + @UserName)
	END

	EXECUTE('CREATE LOGIN ' + @UserName + ' WITH PASSWORD= '''+ @Password +''', DEFAULT_DATABASE= ' + @DBName +', DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF')
	EXECUTE('ALTER LOGIN '  + @UserName + ' ENABLE')

	IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @UserName)
	BEGIN
		EXECUTE('DROP USER ' + @UserName)
	END

	EXECUTE('CREATE USER ' + @UserName + ' FOR LOGIN ' + @UserName + ' WITH DEFAULT_SCHEMA=[dbo]')

	EXEC sp_addrolemember 'db_owner', @UserName
	-------------------------------------------------------------------------
END
GO