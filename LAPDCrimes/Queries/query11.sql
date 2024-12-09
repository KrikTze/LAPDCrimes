WITH areas_with_crime_X AS (

SELECT cr.area_code as area, date_occ,COUNT(crime_code) as counter
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
INNER JOIN "CrimesSchema".crime_codes cr_cod
ON cr_c.crime_code = cr_cod.crm_code
WHERE cr_cod.crm_descr = 'THEFT OF IDENTITY'
GROUP BY area, date_occ
HAVING COUNT(crime_code)>1
ORDER BY counter DESC
),
areas_with_crime_Y AS
(
SELECT cr.area_code as area, date_occ,COUNT(crime_code) as counter
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr.drno = cr_c.drno
INNER JOIN "CrimesSchema".crime_codes cr_cod
ON cr_c.crime_code = cr_cod.crm_code
WHERE cr_cod.crm_descr = 'CHILD ANNOYING (17YRS & UNDER)'
GROUP BY area, date_occ
HAVING COUNT(crime_code)>1

)
SELECT DISTINCT x.area,x.date_occ
FROM areas_with_crime_Y as y
INNER JOIN areas_with_crime_X as x
ON x.area = y.area AND x.date_occ = y.date_occ

-- SELECT cr.area_code as area_code
-- FROM "CrimesSchema".crimes_commited as cr_c
-- INNER JOIN "CrimesSchema".crimes as cr
-- ON cr.drno = cr_c.drno
-- WHERE 