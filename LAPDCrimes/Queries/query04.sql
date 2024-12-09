--query 4 Average Crime Per Hour for a date range
SELECT COUNT(cr_c.crime_code)/(('2020-01-09'::DATE - '2020-01-08'::DATE)+1)  as average_per_hour
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
WHERE  date_occ >= '2020-01-08' AND date_occ <= '2020-01-09' 
GROUP BY LEFT(cr.time_occ, 2)
--GROUP BY cr.date_occ