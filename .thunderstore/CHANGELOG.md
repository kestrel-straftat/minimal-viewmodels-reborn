### v1.3.7

- Added option to mirror the viewmodel (courtesy of [1rubyrain](https://github.com/1rubyrain) in [#1](https://github.com/kestrel-straftat/minimal-viewmodels-reborn/pull/1))
- Temporarily disabled the `Muzzle Flash Light Intensity` option - muzzle flash lights have been broken for several months and nobody noticed. lol
    - `Muzzle Flash Scale` is still enabled and works as intended

### v1.2.7

- Fixed bullet trails ending at the wrong point

### v1.2.6

- Fixed the `Hide Bullet Trails` option hiding bullet trails for everyone if the host enabled it. oops :3

### v1.2.5

- Added option to hide bullet tracers
- The `MuzzleFlashes.General` config section has been removed in favour of a `VFX.MuzzleFlashes` section - your muzzle
flash configs may have been reset (sorry!)

### v1.2.4

- Removed minimal muzzle flash vfx blacklist

### v1.2.3

- Added option to hide arms

### v1.1.3

- Increased range of allowed viewmodel offset values to [-5, 5]
- Fixed modified dynamic fov not applying properly on player spawn

### v1.1.2

- Rewrite of viewmodel repositioning code
- The mod should now generally be more stable in terms of not messing with how items interact with the world
- Melee weapons can now be repositioned!
- Removed per weapon viewmodel positioning. Sorry but like nobody used it, and it caused more issues than it solved :c
- Added options to customise the fov change when sprinting, sliding, and sprint sliding.

### v1.0.2

- Removed unnecessary numbers from beginnings of setting names

### v1.0.0

- Initial release