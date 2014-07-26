USE [EVE]
GO

/****** Object:  StoredProcedure [dbo].[usp_sel_ProductMetaInfo]    Script Date: 7/25/2014 7:51:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_sel_ProductMetaInfo]
(
	@pid INT
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @T TABLE(
		IndexId INT IDENTITY(1,1),
		Id INT,
		MetaId INT,
		NameEn VARCHAR(300),
		NameEs VARCHAR(300),
		OPrice MONEY,
		ORetailPrice MONEY,
		Price MONEY,
		RetailPrice MONEY,
		Tags VARCHAR(MAX),
		TagsEn VARCHAR(MAX),
		TagsEs VARCHAR(MAX),
		Inventory INT,
		InventoryHold INT
	)
	INSERT INTO @t
		SELECT
			P.ProductId,
			PM.ProductMetaId,
			P.ProductNameEn,
			P.ProductNameEs,
			P.ProductPrice,
			P.ProductRetailPrice,
			PM.ProductMetaPrice,
			PM.ProductMetaRetailPrice,
			PM.ProductMetaTags,
			PM.ProductMetaTags,
			PM.ProductMetaTags,
			PM.ProductMetaInventory,
			PM.ProductMetaInventoryHold
		FROM ProductMeta PM
		INNER JOIN Product P ON PM.ProductId = P.ProductId AND P.ProductId = @pid

	DECLARE @M TABLE(
		IndexId INT IDENTITY(1,1),
		Tag VARCHAR(MAX),
		DescEn VARCHAR(MAX),
		DescEs VARCHAR(MAX)
	)
	INSERT INTO @M
		SELECT
			'[' + CAST(M.MetaId AS VARCHAR(999)) + '~' + CAST(MO.MetaOptionId AS VARCHAR(999)) + ']' AS MetaTag,
			'[' + M.MetaNameEn + '~' + MO.MetaOptionNameEn + ']' AS MetaDescEn,
			'[' + M.MetaNameEs + '~' + MO.MetaOptionNameEs + ']' AS MetaDescEs
		FROM MetaOption MO
		INNER JOIN Meta M ON M.MetaId = MO.MetaId

	DECLARE @minM INT, @maxM INT, @tag VARCHAR(MAX), @en VARCHAR(MAX), @es VARCHAR(MAX)
	SELECT @minM = MIN(IndexId), @maxM = MAX(IndexId) FROM @m

	WHILE @minM <= @maxM
	BEGIN
		SELECT @tag = Tag, @en = DescEn, @es = DescEs FROM @m WHERE IndexId = @minM
		UPDATE @t SET
			TagsEn = REPLACE(TagsEn, @tag, @en),
			TagsEs = REPLACE(TagsEs, @tag, @es)
		SET @minM = @minM + 1
	END

	SELECT
		Id ProductId,
		MetaId ProductMetaId,
		NameEn ProductNameEn,
		NameEs ProductNameEs,
		OPrice ProductPrice,
		ORetailPrice ProductRetailPrice,
		Price ProductMetaPrice,
		RetailPrice ProductMetaRetailPrice,
		Tags ProductMetaTags,
		TagsEn ProductMetaTagsEn,
		TagsEs ProductMetaTagsEs,
		Inventory ProductMetaInventory,
		InventoryHold ProductMetaInventoryHold
	FROM @t
	SET NOCOUNT OFF
END
GO

