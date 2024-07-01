package testmod.condfail

import rego.v1

p[x] := v if {
	some i
	x := input.x[i]
	v := input.x[i] + input.y[i]
}
