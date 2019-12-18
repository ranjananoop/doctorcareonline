USE [DOCCAREONLINE]
GO

/****** Object:  Table [dbo].[PAYPALIPNS]    Script Date: 11/09/2013 11:46:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PAYPALIPNS](
	[PAYPALIPNID] [bigint] IDENTITY(1,1) NOT NULL,
	[PAYPALRESPONSE] [nvarchar](max) NULL,
	[CREATEDBY] [nvarchar](100) NULL,
	[CREATEDDATE] [datetime] NULL,
	[MODIFIEDBY] [nvarchar](100) NULL,
	[MODIFIEDDATE] [datetime] NULL,
	[GUID] [varchar](50) NULL,
 CONSTRAINT [PK_PAYPALIPNS] PRIMARY KEY CLUSTERED 
(
	[PAYPALIPNID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [DOCCAREONLINE]
GO

/****** Object:  Table [dbo].[SUBSCRIPTIONPAYMENTS]    Script Date: 11/09/2013 11:46:26 ******/
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
 CONSTRAINT [PK_dbo.SUBSCRIPTIONPAYMENTS] PRIMARY KEY CLUSTERED 
(
	[SUBSCRIPTIONPAYMENTID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [DOCCAREONLINE]
GO

/****** Object:  Table [dbo].[DOCSUBSCRIPTIONS]    Script Date: 11/09/2013 11:46:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DOCSUBSCRIPTIONS](
	[DOCSUBSCRIPTIONID] [bigint] IDENTITY(1,1) NOT NULL,
	[MaxNumberOfPayments] [int] NULL,
	[StartingDate] [datetime] NULL,
	[PinType] [nvarchar](max) NULL,
	[MaxAmountPerPayment] [decimal](18, 2) NULL,
	[CurrencyCode] [nvarchar](max) NULL,
	[SenderEmail] [nvarchar](max) NULL,
	[VerifySign] [nvarchar](max) NULL,
	[TestIpn] [nvarchar](max) NULL,
	[DateOfMonth] [int] NULL,
	[CurrentNumberOfPayments] [int] NULL,
	[PreapprovalKey] [nvarchar](max) NULL,
	[EndingDate] [datetime] NULL,
	[IsApproved] [bit] NULL,
	[TransactionType] [nvarchar](max) NULL,
	[DayOfWeek] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NULL,
	[CurrentTotalAmountOfAllPayments] [decimal](18, 2) NULL,
	[CurrentPeriodAttempts] [int] NULL,
	[Charset] [nvarchar](max) NULL,
	[PaymentPeriod] [nvarchar](max) NULL,
	[NotifyVersion] [nvarchar](max) NULL,
	[MaxTotalAmountOfAllPayments] [decimal](18, 2) NULL,
	[ERRORDETAILS] [nvarchar](max) NULL,
	[UserName] [nvarchar](100) NULL,
	[CREATEDDATE] [datetime] NULL,
	[CREATEDBY] [nvarchar](100) NULL,
	[MODIFIEDDATE] [datetime] NULL,
	[MODIFIEDBY] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.DOCSUBSCRIPTIONS] PRIMARY KEY CLUSTERED 
(
	[DOCSUBSCRIPTIONID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SUBSCRIPTIONPAYMENTS]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SUBSCRIPTIONPAYMENTS_dbo.USERS_UserName] FOREIGN KEY([UserName])
REFERENCES [dbo].[USERS] ([UserName])
GO

ALTER TABLE [dbo].[SUBSCRIPTIONPAYMENTS] CHECK CONSTRAINT [FK_dbo.SUBSCRIPTIONPAYMENTS_dbo.USERS_UserName]
GO

ALTER TABLE [dbo].[DOCSUBSCRIPTIONS]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DOCSUBSCRIPTIONS_dbo.USERS_UserName] FOREIGN KEY([UserName])
REFERENCES [dbo].[USERS] ([UserName])
GO

ALTER TABLE [dbo].[DOCSUBSCRIPTIONS] CHECK CONSTRAINT [FK_dbo.DOCSUBSCRIPTIONS_dbo.USERS_UserName]
GO


