var DecayExport = function(global)
{
	var output = "";
	output += "<?xml version=\"1.0\"?>\n";
	output += "<resources xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n";

	for (var i = 0; i < global.bodies.length; i++) {
		var body = global.bodies[i];

		if (body.fixtures.length < 1) continue;
		var fixture = body.fixtures[0];

		if (fixture.isCircle || fixture.hull.length < 1) continue;
		var hull = fixture.hull;

		output += "    <collider2d id=\"" + body.name + "\">\n";
		output += "        <properties>\n";
		output += "            <mass>" + body.mass + "</mass>\n";
		output += "            <margin>" + body.margin + "</margin>\n";
		output += "            <friction>\n";
		output += "                <linear>" + body.frictionLinear + "</linear>\n";
		output += "                <spinning>" + body.frictionSpinning + "</spinning>\n";
		output += "                <rolling>" + body.frictionRolling + "</rolling>\n";
		output += "            </friction>\n";
		output += "            <anisotropicFriction x=\"" + body.frictionAnisotropicX + "\" y=\"" + body.frictionAnisotropicY + "\" z=\"" + body.frictionAnisotropicZ + "\" />\n";
		output += "            <damping>\n";
		output += "                <linear>" + body.dampingLinear + "</linear>\n";
		output += "                <angular>" + body.dampingAngular + "</angular>\n";
		output += "            </damping>\n";
		output += "            <sleepingThereshold>\n";
		output += "                <linear>" + body.sleepingTheresholdLinear + "</linear>\n";
		output += "                <angular>" + body.sleepingTheresholdAngular + "</angular>\n";
		output += "            </sleepingThereshold>\n";
		output += "            <restitution>" + body.restitution + "</restitution>\n";
		output += "            <hull>\n";

		for (var j = 0; j < hull.length; j++) {
			var width = body.size.width;
			var height = body.size.height;
			var xAspectRatioCorrectionFactor = 1;
			var yAspectRatioCorrectionFactor = 1;
			if (width > height) {
				xAspectRatioCorrectionFactor = width / height;
			} else if (width < height) {
				yAspectRatioCorrectionFactor = width / height;
			}

			var x = (hull[j].x / width) * yAspectRatioCorrectionFactor;
			var y = (hull[j].y / height) * xAspectRatioCorrectionFactor;

			output += "                <vertex x=\"" + x + "\" y=\"" + y + "\" />\n";
		}

		output += "            </hull>\n";
		output += "        </properties>\n";
		output += "    </collider2d>\n";
	}

	output += "</resources>";
	return output;
};
DecayExport.filterName = "DecayExport";
Library.addFilter("DecayExport");
