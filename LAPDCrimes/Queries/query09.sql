WITH wpns_per_age AS
(SELECT wpn_code, CONCAT(5*FLOOR(victim_age/5),' <= x < ',5*FLOOR(victim_age/5)+5) as age_buckets, COUNT(wpn_code) as wpn_count
FROM "CrimesSchema".crimes_commited as cr_c
INNER JOIN "CrimesSchema".crimes as cr
ON cr_c.drno = cr.drno
INNER JOIN "CrimesSchema".crimes_victims as cr_vic
ON cr.drno = cr_vic.drno
INNER JOIN "CrimesSchema".victims as vic
ON vic.victim_id = cr_vic.victim_id
WHERE wpn_code IS NOT NULL
GROUP BY wpn_code, FLOOR(victim_age/5)

SELECT age_buckets, MAX(wpn_count)
FROM wpns_per_age
GROUP BY age_buckets

