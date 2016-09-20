# MaxDistancePatch

> **Removes legacy object visibility limits in [MaSzyna](http://eu07.pl) train simulator.**

The distance the objects are visible from was set up in scenery files. This allowed the simulator to display only those object which are not too far from the camera. The authors who made sceneries many years ago on legacy computers set those values so low to cause visible and disturbing visual artifact of objects jumping suddelny into view. On some sceneries, like Drawinowo - the lanscape beyond the windshield looked a little desert-like. The current version of the simulator calculates objects visibility automatically, unless the fixed value is set in a scenery file. This program removes all fixed visibility limits and allows the simulator to decide what should be rendered based on dynamic calculations.

> **To apply:**

* Copy `MaxDistancePatch.exe` and `pl-PL` directory to main `MaSzyna` directory.
* Run `MaxDistancePatch.exe`.
* Wait until the files are scanned.
* Click the "PATCH" button.

> **To test:**

* Start "Drawinowo" scenery before and after applying the patch.
* Drive and admire the better views with patched version.

> **To Remove:**

* Enter `MaxDistancePatch.Backup` directory.
* Select `scenery` directory.
* Cut it with `Ctrl+X`.
* Enter `MaSzyna` main directory.
* Paste backup with `Ctrl+V`, select "Replace all".
* Remove `MaxDistancePatch.Backup` directory.
* Remove `MaxDistancePatch` files.

> **But is it safe?**
>
> I tested it. Nothing will break. I promise ;)