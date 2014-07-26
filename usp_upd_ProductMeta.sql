USE [EVE]
GO

/****** Object:  StoredProcedure [dbo].[usp_upd_ProductMeta]    Script Date: 7/25/2014 7:51:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_upd_ProductMeta]
(
	@x XML
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @t TABLE(IndexId INT IDENTITY(1,1), ProductId INT, Meta VARCHAR(MAX), MetaId INT) 
	DECLARE @d TABLE(IndexId INT IDENTITY(1,1), ProductMetaId INT)
	DECLARE @r TABLE(Reason VARCHAR(MAX))

	INSERT INTO @t
	SELECT Tbl.Col.value('@pid', 'int'), Tbl.Col.value('@tags', 'varchar(max)'), 0 FROM @x.nodes('//m') Tbl(Col)

	DECLARE @min INT, @max INT, @del INT, @tid INT
	SELECT @min = MIN(IndexId), @max = MAX(IndexId) FROM @t

	WHILE @min <= @max
	BEGIN
		SET @tid = -1
		--Find the corresponding id based on its meta definition
		SELECT @tid = PM.ProductMetaId FROM ProductMeta PM INNER JOIN @t T ON
			T.ProductId = PM.ProductId AND PM.ProductMetaTags = T.Meta AND T.IndexId = @min
		UPDATE @T SET MetaId  = @tid WHERE IndexId = @min
		SET @min = @min + 1
	END

	--find the ones that should be deleted since there are no longer in the coleection
	INSERT INTO @d
		SELECT ProductMetaId FROM ProductMeta WHERE ProductMetaId NOT IN (SELECT MetaId FROM @t)

	--Insert new metas
	INSERT INTO ProductMeta
		SELECT T.ProductId, T.Meta, NULL, NULL, 0, 0 FROM @t T WHERE MetaId = -1

	SELECT @min = MIN(IndexId), @max = MAX(IndexId) FROM @d

	WHILE @min <= @max
	BEGIN
		SET @del = -1
		SELECT @del = ProductMetaId FROM @d WHERE IndexId = @min

		BEGIN TRY
			--Try to delete it
			DELETE FROM ProductMeta WHERE ProductMetaId = @del
		END TRY
		BEGIN CATCH
			--Log failure
			INSERT INTO @r VALUES('Unable to delete ProductMeta ' + CAST(@del AS VARCHAR(99)))
		END CATCH
		SET @min = @min + 1
	END

	--Select failures
	SELECT * FROM @r

	SET NOCOUNT OFF
END
GO

