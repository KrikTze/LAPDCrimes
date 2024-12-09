SELECT crime_code,date_occ, COUNT(crime_code) as counter
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
WHERE cr_c.crime_code = '998' AND date_occ >= '2020-01-08' AND date_occ <= '2020-01-09' 
GROUP BY crime_code,date_occ
