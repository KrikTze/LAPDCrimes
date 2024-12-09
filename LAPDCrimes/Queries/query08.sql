WITH pairs as (SELECT  cr_c.crime_code as cr1 ,cr_c2.crime_code as cr2,COUNT(cr_c2.crime_code) AS counter
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes_commited as cr_c2
ON cr_c.drno = cr_c2.drno
INNER JOIN "CrimesSchema".crimes as cr
ON cr_c.drno = cr.drno
WHERE cr_c.crime_code != cr_c2.crime_code AND cr_c2.crime_code = '998' and cr.date_occ >= '2020-01-08' AND cr.date_occ <= '2020-01-20' 
GROUP BY cr_c.crime_code,cr_c2.crime_code
ORDER BY counter DESC)

SELECT cr1
FROM pairs
WHERE pairs.counter != (SELECT MAX(counter) from pairs)
ORDER BY counter DESC
LIMIT 1