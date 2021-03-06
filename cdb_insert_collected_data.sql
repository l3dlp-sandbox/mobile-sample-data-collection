CREATE OR REPLACE FUNCTION cdb_insert_collected_data(jsons jsonb[])
 RETURNS TABLE(cartodb_id integer)
 LANGUAGE plpgsql
 STRICT SECURITY DEFINER
AS $function$

DECLARE

sql text;

device_identifier text;
title text;
description text;
attachment_url text;
longitude float;
latitude float;
user_longitude float;
user_latitude float;
user_accuracy float;
the_geom text;
marker_latitude float;
marker_longitude float;

report_time_double double precision;
report_time timestamp with time zone;

keys text;
values text;

BEGIN

keys := 'the_geom,device_identifier,title,description,user_accuracy,user_latitude,user_longitude,report_time,attachment_url';
values := '';

FOR i in 1 .. array_upper(jsons, 1) LOOP
  IF i > 1 THEN 
    values := values || ','; 
  END IF;

  latitude := jsons[i]::json->>'marker_latitude';
  longitude := jsons[i]::json->>'marker_longitude';
  
  the_geom := 'CDB_LatLng(' || latitude || ',' || longitude || ')';
  
  device_identifier := jsons[i]::json->>'device_identifier';
  title := jsons[i]::json->>'title';
  description := jsons[i]::json->>'description';
  attachment_url := jsons[i]::json->>'attachment_url';
  user_accuracy := jsons[i]::json->>'user_accuracy';
  user_longitude := jsons[i]::json->>'user_longitude';
  user_latitude := jsons[i]::json->>'user_latitude';

  report_time_double := jsons[i]::json->>'report_time';
  report_time := to_timestamp(report_time_double) AT TIME ZONE 'UTC';
  
  values := values || '(';
  values := values || the_geom || ',';
  values := values || quote_literal(device_identifier) || ',';
  values := values || quote_literal(title) || ',';
  values := values || quote_literal(description) || ',';
  values := values || quote_literal(user_accuracy) || ',';
  values := values || quote_literal(user_latitude) || ',';
  values := values || quote_literal(user_longitude) || ',';
  values := values || quote_literal(report_time) || ',';
  values := values || quote_literal(attachment_url);
  
  values := values || ')';
  
END LOOP;

sql := 'WITH do_insert AS (INSERT INTO sample_collected_data (' || keys || ') '
  || 'VALUES '|| values
  || 'RETURNING cartodb_id) SELECT cartodb_id FROM do_insert';

RAISE DEBUG '%', sql;

RETURN QUERY EXECUTE sql;

END;
$function$
