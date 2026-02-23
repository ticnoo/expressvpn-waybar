
```bash
git clone https://github.com/ticnoo/expressvpn-waybar.git
```

# build

```bash
./build.sh
```
```bash
which expressvpn-waybar
```

# Usage

```conf
"custom/expressvpn": {
		"tooltip": true,
        "return-type": "json",
        "format": "{}",
		"exec": "expressvpn-waybar",
        "on-click": "expressvpn-waybar --toggle",
        "on-click-right": "/opt/expressvpn/bin/expressvpn-client &"
    },
```
