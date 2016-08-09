# TargetGenerator

## Descend Via
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | dv \<star> \<rwy> | dv robuc2 22l | Descend via with STAR and runway assignment.
:x: | dv \<star> | dv robuc2 | Descend via with STAR assignment.
:x: | dv \<rwy> | dv 22l | Descend via with runway assignment.
:x: | dv | dv | Descend via previously assigned STAR.

## Depart Heading
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | dh \<waypoint> \<heading> | dh rever 180 | Depart waypoint with heading.
:x: | dh \<heading> | dh 180 | Descend next waypoint with heading.

## Descend at Waypoint
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | ad \<waypoint> \<altitude> | ad rever 060 | Descend to altitude at specified waypoint.
:x: | ad \<heading> | ad 060 | Descend to altitude at next waypoint.

## Visual Separation
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | mvs \<callsign> | mvs swa291 | Maintain visual separations from the specified traffic.

## Heading Instructions
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | tdl | tdl \<degrees> | Turn specified number of degrees left.
:x: | tdr | tdr \<degrees> | Turn specified number of degrees right.
:x: | fph | fph | Fly present heading.

## Direct
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | pd | pd \<waypoint> | Proceed direct to a waypoint.
:x: | cd | cd \<waypoint> | Cleared direct to a waypoint.
:x: | wapd | wapd \<waypoint> | When able, proceed direct to a waypoint.

## Compound Instructions
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | pd | pd \<waypoint> | Proceed direct to a waypoint.
:x: | cd | cd \<waypoint> | Cleared direct to a waypoint.

## Radials
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | pd | pd \<waypoint> | Proceed direct to a waypoint.
:x: | cd | cd \<waypoint> | Cleared direct to a waypoint.

## Joining Paths
Status | Command | Sample | Description
--- | --- | --- | ---
:x: | jri | jri \<waypoint> \<radial> | Join the radial inbound the waypoint.
:x: | jro | jro \<waypoint> \<radial> | Join the radial outbound the waypoint.
:x: | jl | jl \<runway> | Join the localizer.



