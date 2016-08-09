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
:x: | tdl \<degrees> | tdl \<degrees> | Turn specified number of degrees left.
:x: | tdr \<degrees> | tdr \<degrees> | Turn specified number of degrees right.
:x: | fph | fph | Fly present heading.
:x: | dh \<waypoint> \<heading> | dh rever 180 | Depart waypoint with heading.
:x: | pd \<waypoint> | pd jfk | Proceed direct to a waypoint.
:x: | cd \<waypoint> | cd bos | Cleared direct to a waypoint.
:x: | wapd \<waypoint> | wapd mht | When able, proceed direct to a waypoint.
:x: | jri \<waypoint> \<radial> | jri bos 180 | Join the radial inbound the waypoint.
:x: | jro \<waypoint> \<radial> | jro bos 270 | Join the radial outbound the waypoint.
:x: | jl \<runway> | jl 27 | Join the localizer.

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

