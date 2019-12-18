USE [DOCCAREONLINE]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dbo.SUBSCRIPTIONPAYMENTS_dbo.USERS_UserName]') AND parent_object_id = OBJECT_ID(N'[dbo].[SUBSCRIPTIONPAYMENTS]'))
ALTER TABLE [dbo].[SUBSCRIPTIONPAYMENTS] DROP CONSTRAINT [FK_dbo.SUBSCRIPTIONPAYMENTS_dbo.USERS_UserName]
GO

USE [DOCCAREONLINE]
GO

/****** Object:  Table [dbo].[SUBSCRIPTIONPAYMENTS]    Script Date: 11/11/2013 10:01:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SUBSCRIPTIONPAYMENTS]') AND type in (N'U'))
DROP TABLE [dbo].[SUBSCRIPTIONPAYMENTS]
GO

USE [DOCCAREONLINE]
GO

/****** Object:  Table [dbo].[SUBSCRIPTIONPAYMENTS]    Script Date: 11/11/2013 10:01:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SUBSCRIPTIONPAYMENTS](
	[SUBSCRIPTIONPAYMENTID] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionSubject] [nvarchar](max) NULL,
	[PaymentDate] [nvarchar](50) NULL,
	[TxnType] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[ResidenceCountry] [nvarchar](max) NULL,
	[ItemName] [nvarchar](max) NULL,
	[PaymentGross] [decimal](18, 2) NULL,
	[McCurrency] [nvarchar](max) NULL,
	[Business] [nvarchar](max) NULL,
	[PaymentType] [nvarchar](max) NULL,
	[ProtectionEligibility] [nvarchar](max) NULL,
	[VerifySign] [nvarchar](max) NULL,
	[PayerStatus] [nvarchar](max) NULL,
	[TestIpn] [nvarchar](max) NULL,
	[Tax] [nvarchar](max) NULL,
	[PayerEmail] [nvarchar](max) NULL,
	[TxnId] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[ReceiverEmail] [nvarchar](max) NULL,
	[FirstName] [nvarchar](max) NULL,
	[PayerId] [nvarchar](max) NULL,
	[ReceiverId] [nvarchar](max) NULL,
	[ItemNumber] [nvarchar](max) NULL,
	[PaymentStatus] [nvarchar](max) NULL,
	[PaymentFee] [decimal](18, 2) NULL,
	[McFee] [decimal](18, 2) NULL,
	[McGross] [decimal](18, 2) NULL,
	[Custom] [nvarchar](max) NULL,
	[Charset] [nvarchar](max) NULL,
	[NotifyVersion] [nvarchar](max) NULL,
	[IpnTrackId] [nvarchar](max) NULL,
	[PendingReason] [nvarchar](max) NULL,
	[ERRORDETAILS] [nvarchar](max) NULL,
	[UserName] [nvarchar](100) NULL,
	[CREATEDDATE] [datetime] NULL,
	[CREATEDBY] [nvarchar](100) NULL,
	[MODIFIEDDATE] [datetime] NULL,
	[MODIFIEDBY] [nvarchar](100) NULL,
	[PreapprovalKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.SUBSCRIPTIONPAYMENTS] PRIMARY KEY CLUSTERED 
(
	[SUBSCRIPTIONPAYMENTID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SUBSCRIPTIONPAYMENTS]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SUBSCRIPTIONPAYMENTS_dbo.USERS_UserName] FOREIGN KEY([UserName])
REFERENCES [dbo].[USERS] ([UserName])
GO

ALTER TABLE [dbo].[SUBSCRIPTIONPAYMENTS] CHECK CONSTRAINT [FK_dbo.SUBSCRIPTIONPAYMENTS_dbo.USERS_UserName]
GO


