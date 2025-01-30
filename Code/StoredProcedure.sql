-- Example of creating a stored procedure that could be used to fetch the correct SQL view with user-given parameters. This was used as part of internal website functionality where a user could give parameters and then get an Excel report based on the input.

CREATE PROCEDURE sp_ViewReport_FetchReportData
@reportId INT,
@advertiserId INT = NULL,
@mediaAgencyId INT = NULL,
@startDate DATETIME = NULL,
@endDate DATETIME = NULL
as
BEGIN
	DECLARE @useDateFiltering bit, @useAdvertiserFiltering bit, @useMediaAgencyFiltering bit, @command nvarchar(10), @viewName nvarchar(50), @viewReportQuery nvarchar(MAX), @parameters nvarchar(MAX);

	SET @useAdvertiserFiltering = (SELECT useAdvertiserFiltering FROM dev.dbo.VIEWREPORT WHERE reportId = @reportId);
	SET @useMediaAgencyFiltering = (SELECT useMediaAgencyFiltering FROM dev.dbo.VIEWREPORT WHERE reportId = @reportId);
	SET @useDateFiltering = (SELECT useDateFiltering FROM dev.dbo.VIEWREPORT WHERE reportId = @reportId);
	SET @viewName = (SELECT viewName FROM dev.dbo.VIEWREPORT WHERE reportId = @reportId);
	SET @viewReportQuery = 'SELECT * FROM ' + @viewName;

	SET @command = ' WHERE ';

	IF (@useAdvertiserFiltering = 1)
		BEGIN
			SET @viewReportQuery = @viewReportQuery + @command + 'advertiserID = @advertiserId';
			SET @command = ' AND ';
		END
	IF (@useMediaAgencyFiltering = 1)
		BEGIN
			SET @viewReportQuery = @viewReportQuery + @command + 'mediaAgencyID = @mediaAgencyId';
			SET @command = ' AND ';
		END
	IF (@useDateFiltering = 1)
		BEGIN
			SET @viewReportQuery = @viewReportQuery + @command + 'startDate BETWEEN @startDate AND @endDate';			
		END

	SET @parameters = '@reportId INT, @advertiserId INT, @mediaAgencyID INT, @startDate DATETIME, @endDate DATETIME';

	EXEC sys.sp_executesql @viewReportQuery, @parameters, @reportId = @reportId, @advertiserId = @advertiserId, @mediaAgencyId = @mediaAgencyId, @startDate = @startDate, @endDate = @endDate
END
