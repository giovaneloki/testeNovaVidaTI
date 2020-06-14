USE [master]
GO
/****** Object:  Database [dbTesteNovaVidaTI]    Script Date: 14/06/2020 20:32:37 ******/
CREATE DATABASE [dbTesteNovaVidaTI]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'dbTesteNovaVidaTI', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\dbTesteNovaVidaTI.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'dbTesteNovaVidaTI_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\dbTesteNovaVidaTI_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dbTesteNovaVidaTI].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET ARITHABORT OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET  ENABLE_BROKER 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET  MULTI_USER 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET DB_CHAINING OFF 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET QUERY_STORE = OFF
GO
USE [dbTesteNovaVidaTI]
GO
/****** Object:  Table [dbo].[Aluno]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Aluno](
	[IdAluno] [int] IDENTITY(1,1) NOT NULL,
	[IdProfessor] [int] NULL,
	[Nome] [varchar](255) NULL,
	[Mensalidade] [decimal](10, 2) NULL,
	[DataVencimento] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Importacao]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Importacao](
	[IdImportacao] [int] IDENTITY(1,1) NOT NULL,
	[NomeArquivo] [varchar](255) NULL,
	[Tamanho] [int] NULL,
	[QtdRegistro] [int] NULL,
	[QtdImportado] [int] NULL,
	[DataImportacao] [datetime] NULL,
	[IdProfessor] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Professor]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Professor](
	[IdProfessor] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](255) NULL,
	[DataInclusao] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Importacao] ADD  DEFAULT (getdate()) FOR [DataImportacao]
GO
ALTER TABLE [dbo].[Professor] ADD  DEFAULT (getdate()) FOR [DataInclusao]
GO
/****** Object:  StoredProcedure [dbo].[sp_AddArquivoImportacao]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[sp_AddArquivoImportacao]
(
	@IdProfessor INT,
	@NomeArquivo VARCHAR(255),
	@Tamanho INT,
	@QtdLinhas INT,
	@QtdImportado INT
)
AS
BEGIN
	INSERT INTO Importacao(IdProfessor, NomeArquivo, Tamanho, QtdRegistro, QtdImportado)
	VALUES(@IdProfessor, @NomeArquivo, @Tamanho, @QtdLinhas, @QtdImportado)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DataUltimaImportacao]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_DataUltimaImportacao]
(
	@IdProfessor INT
)
AS
BEGIN
	SELECT MAX(DataImportacao) FROM Importacao WHERE IdProfessor = @IdProfessor
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ListarAlunos]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_ListarAlunos]
(
	@IdProfessor INT
)
AS
BEGIN
	SELECT * FROM Aluno WHERE IdProfessor = @IdProfessor
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ListarProfessores]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_ListarProfessores]
AS
BEGIN
	SELECT * FROM Professor
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PermiteImportacao]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_PermiteImportacao]
(
	@Tempo INT,
	@IdProfessor INT
)
AS
BEGIN
	IF((SELECT COUNT(1) FROM Importacao WHERE IdProfessor = @IdProfessor) = 0)
	BEGIN
		SELECT 1 Permite
	END
	ELSE
	BEGIN
		SELECT CASE WHEN (GETDATE() > DATEADD(SECOND, @Tempo, MAX(DataImportacao))) THEN 1 ELSE 0 END
			FROM Importacao WHERE IdProfessor = @IdProfessor
	END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoverAluno]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_RemoverAluno]
(
	@IdAluno INT
)
AS
BEGIN
	DELETE FROM Aluno WHERE IdAluno = @IdAluno
END
GO
/****** Object:  StoredProcedure [dbo].[spAddProfessor]    Script Date: 14/06/2020 20:32:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[spAddProfessor]
(
	@Nome VARCHAR(255)
)
AS
BEGIN
	INSERT INTO Professor(Nome)
	VALUES(@Nome)
END
GO
USE [master]
GO
ALTER DATABASE [dbTesteNovaVidaTI] SET  READ_WRITE 
GO
