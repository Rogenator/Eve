USE [EVE]
GO

/****** Object:  StoredProcedure [dbo].[usp_upd_MetaOptions]    Script Date: 7/25/2014 7:51:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_upd_MetaOptions]
(
	@x XML
)
AS
BEGIN
	DECLARE @t TABLE(IndexId INT IDENTITY(1,1), mid INT, oid INT, es VARCHAR(50), en VARCHAR(50), val VARCHAR(50), rid INT)
	DECLARE @d TABLE(oid INT)
	DECLARE @min INT, @max INT, @mid INT, @oid INT

	INSERT INTO @t
	SELECT
		Tbl.Col.value('@mid', 'int'),
		Tbl.Col.value('@oid', 'int'),
		Tbl.Col.value('@es', 'varchar(50)'),
		Tbl.Col.value('@en', 'varchar(50)'),
		Tbl.Col.value('@val', 'varchar(50)'),
		-1
	FROM @x.nodes('//m') Tbl(Col)

	SELECT @min = MIN(IndexId), @max = MAX(IndexId) FROM @t
	SELECT TOP 1 @mid = mid FROM @T

	INSERT INTO @d
		SELECT MetaOptionId FROM MetaOption WHERE MetaOptionId NOT IN (SELECT oid FROM @t) AND MetaId = @mid

	WHILE @min <= @max
	BEGIN
		SELECT @oid = oid FROM @t WHERE IndexId = @min
		IF @oid = -1
		BEGIN
			INSERT INTO MetaOption SELECT mid, es, en, val FROM @t WHERE IndexId = @min
		END
		ELSE
		BEGIN
			UPDATE MetaOption SET
				MetaOption.MetaOptionNameEn = T.en,
				MetaOption.MetaOptionNameEs = T.es,
				MetaOption.MetaOptionValue = T.val
			FROM MetaOption
			INNER JOIN @t T ON T.IndexId = @min AND MetaOption.MetaOptionId = @oid
		END
		SET @min = @min + 1
	END

	DELETE FROM MetaOption WHERE MetaOptionId IN (SELECT oid FROm @d)
END
GO

