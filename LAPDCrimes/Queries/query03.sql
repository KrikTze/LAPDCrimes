WITH x AS (
	SELECT area_code,crime_code,count(crime_code) as counter
	FROM "CrimesSchema".crimes_commited as cr_c
	INNER JOIN "CrimesSchema".crimes as cr
	ON cr.drno = cr_c.drno
	WHERE cr.date_occ = '2020-01-08'
	GROUP BY area_code,crime_code
),
 maximum_table AS 
(SELECT  area_code, MAX(x.counter) as maximum
FROM x
GROUP BY area_code )

SELECT maximum_table.area_code, x.crime_code, maximum
FROM x 
INNER JOIN maximum_table
ON maximum = x.counter AND maximum_table.area_code = x.area_code

