---query07
-- 1 find the area
With x as(
SELECT cr.area_code as area_code
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
WHERE  date_occ >= '2020-01-08' AND date_occ <= '2020-01-20' 
GROUP BY cr.area_code
ORDER BY COUNT(crime_code) DESC
LIMIT 1), 

-- find all the pairs for a drno
pairs as(
SELECT  cr_c.crime_code as cr1 ,cr_c2.crime_code as cr2,COUNT(cr_c2.crime_code) AS counter
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes_commited as cr_c2
ON cr_c.drno = cr_c2.drno
INNER JOIN "CrimesSchema".crimes as cr
ON cr_c.drno = cr.drno
INNER JOIN x
ON cr.area_code = x.area_code
WHERE cr_c.crime_code != cr_c2.crime_code and cr.date_occ >= '2020-01-08' AND cr.date_occ <= '2020-01-20' 
GROUP BY cr_c.crime_code,cr_c2.crime_code
ORDER BY counter DESC
LIMIT 1)



SELECT cr1, cr2
from pairs

-- SELECT cr_c.crime_code, COUNT(crime_code)
-- FROM "CrimesSchema".crimes_commited as cr_c
-- INNER JOIN "CrimesSchema".crimes as cr
-- ON cr.drno = cr_c.drno
-- WHERE  date_occ >= '2020-01-08' AND date_occ <= '2020-01-20' and cr.area_code = '01'
-- GROUP BY cr_c.crime_code