SELECT ar.area_name, cr.area_code,COUNT(crime_code) as counter
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
INNER JOIN "CrimesSchema".areas as ar
ON cr.area_code = ar.area_code
WHERE  date_rptd >= '2020-01-08' AND date_rptd <= '2020-01-09' 
GROUP BY cr.area_code , ar.area_name
ORDER BY counter DESC
LIMIT 5