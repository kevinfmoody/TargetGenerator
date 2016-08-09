# TargetGenerator

## Visual Separation
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | mvs \<callsign> | mvs swa291 | Maintain visual separations from the specified traffic.

## Vectoring
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | tlh \<heading> | tlh 200 | Turn left to the specified heading.
:x: | trh \<heading> | trh 140 | Turn right to the specified heading.
:x: | fh \<heading> | fh 180 | Fly the specified heading.
:x: | tdl \<degrees> | tdl 20 | Turn specified number of degrees left.
:x: | tdr \<degrees> | tdr 10 | Turn specified number of degrees right.
:x: | fph | fph | Fly present heading.
:x: | dh \<waypoint> \<heading> | dh rever 180 | Depart waypoint with heading.
:x: | pd \<waypoint> | pd jfk | Proceed direct to a waypoint.
:x: | cd \<waypoint> | cd bos | Cleared direct to a waypoint.
:x: | wapd \<waypoint> | wapd mht | When able, proceed direct to a waypoint.
:x: | jri \<waypoint> \<radial> | jri bos 180 | Join the radial inbound the waypoint.
:x: | jro \<waypoint> \<radial> | jro bos 270 | Join the radial outbound the waypoint.
:x: | jl \<runway> | jl 27 | Join the localizer.

## Altitude Assignment
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | cm \<altitude> | cm 9000 | Climb and maintain the specified altitude.
:x: | dm \<altitude> | dm 040 | Descend and maintain the specified altitude.
:x: | caa \<waypoint> \<altitude> | caa nabbo 030 | Cross the waypoint at or above the specified altitude.
:x: | cab \<waypoint> \<altitude> | cab hfd 6000 | Cross the waypoint at or below the specified altitude.
:x: | cadm \<waypoint> \<altitude> \<altitude> | cadm pvd 110 070 | Cross the waypoint at the first specified altitude, then descend to the second specified altitude.
:x: | cacm \<waypoint> \<altitude> \<altitude> | cacm mht 050 090 | Cross the waypoint at the first specified altitude, then climb to the second specified altitude.
:x: | cam \<waypoint> \<altitude> | cam pvd 110 | Cross the waypoint at and maintain the specified altitude.
:x: | acm \<waypoint> \<altitude> | acm mht 140 | At the waypoint, climb and maintain the specified altitude.
:x: | adm \<waypoint> \<altitude> | adm pvd 070 | At the waypoint, descend and maintain the specified altitude.

## Descend Via
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | dv \<star> \<rwy> | dv robuc2 22l | Descend via with STAR and runway assignment.
:x: | dv \<star> | dv robuc2 | Descend via with STAR assignment.
:x: | dv \<rwy> | dv 22l | Descend via with runway assignment.
:x: | dv | dv | Descend via previously assigned STAR.

## Descend at Waypoint
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | ad \<waypoint> \<altitude> | ad rever 060 | Descend to altitude at specified waypoint.
:x: | ad \<heading> | ad 060 | Descend to altitude at next waypoint.

