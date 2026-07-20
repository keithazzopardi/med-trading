IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    IF SCHEMA_ID(N'Trading') IS NULL EXEC(N'CREATE SCHEMA [Trading];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE TABLE [Trading].[Instruments] (
        [Id] bigint NOT NULL IDENTITY,
        [Symbol] nvarchar(10) NOT NULL,
        [CurrencyCode] nvarchar(5) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [DefaultProvider] nvarchar(50) NOT NULL,
        [Price] decimal(18,6) NOT NULL DEFAULT 0.0,
        [Volume24h] decimal(18,6) NOT NULL DEFAULT 0.0,
        [VolumeLastUpdatedOn] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [UpdatedOn] datetime2 NOT NULL DEFAULT '2026-07-20T09:30:28.7713050Z',
        CONSTRAINT [PK_Instruments] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Instruments_Symbol] UNIQUE ([Symbol])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE TABLE [Trading].[UserPortfolios] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [InstrumentSymbol] nvarchar(10) NULL,
        [AvailableQuantity] decimal(18,6) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_UserPortfolios] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserPortfolios_Instruments_InstrumentSymbol] FOREIGN KEY ([InstrumentSymbol]) REFERENCES [Trading].[Instruments] ([Symbol]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE TABLE [Trading].[TradeOrders] (
        [Id] int NOT NULL IDENTITY,
        [InstrumentSymbol] nvarchar(10) NULL,
        [Side] nvarchar(15) NOT NULL,
        [OrderType] nvarchar(15) NOT NULL,
        [Quantity] decimal(18,6) NOT NULL,
        [Price] decimal(18,6) NOT NULL,
        [UserId] bigint NOT NULL,
        [PortfolioId] bigint NOT NULL,
        [CommissionMoney] decimal(18,6) NOT NULL,
        [Status] nvarchar(15) NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [FilledOn] datetime2 NULL,
        CONSTRAINT [PK_TradeOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TradeOrders_Instruments_InstrumentSymbol] FOREIGN KEY ([InstrumentSymbol]) REFERENCES [Trading].[Instruments] ([Symbol]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TradeOrders_UserPortfolios_PortfolioId] FOREIGN KEY ([PortfolioId]) REFERENCES [Trading].[UserPortfolios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InstrumentEntity_IsActive] ON [Trading].[Instruments] ([UpdatedOn], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InstrumentEntity_VolumeLastUpdatedOn_IsActive] ON [Trading].[Instruments] ([VolumeLastUpdatedOn], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Instruments_Symbol] ON [Trading].[Instruments] ([Symbol]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TradeOrders_InstrumentSymbol] ON [Trading].[TradeOrders] ([InstrumentSymbol]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TradeOrders_PortfolioId] ON [Trading].[TradeOrders] ([PortfolioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_UserPortfolios_InstrumentSymbol] ON [Trading].[UserPortfolios] ([InstrumentSymbol]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_UserPortfolio_UserId_InstrumentSymbol] ON [Trading].[UserPortfolios] ([UserId], [InstrumentSymbol]) WHERE [InstrumentSymbol] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260720093029_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260720093029_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

