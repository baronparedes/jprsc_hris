USE [JPRSC.HRIS]
GO

/****** Object:  Table [dbo].[CompanyClientTags]    Script Date: 10/10/2024 10:17:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CompanyClientTags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.CompanyClientTags] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CompanyClientTags]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CompanyClientTags_dbo.Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Clients] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CompanyClientTags] CHECK CONSTRAINT [FK_dbo.CompanyClientTags_dbo.Clients_ClientId]
GO

ALTER TABLE [dbo].[CompanyClientTags]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CompanyClientTags_dbo.Companies_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Companies] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CompanyClientTags] CHECK CONSTRAINT [FK_dbo.CompanyClientTags_dbo.Companies_CompanyId]
GO


