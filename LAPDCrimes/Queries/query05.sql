SELECT cr_c.crime_code, COUNT(*) AS crime_count
FROM "CrimesSchema".crimes AS cr
INNER JOIN "CrimesSchema".crimes_commited AS cr_c
ON cr.drno = cr_c.drno
INNER JOIN "CrimesSchema".location AS loc 
ON cr.loc_id = loc.loc_id
WHERE loc.loc_point && ST_MakeEnvelope(-118.5, 33.7, -118.1, 34.3, 4326) AND cr.date_occ = '2020-01-09'
GROUP BY cr_c.crime_code
ORDER BY crime_count DESC