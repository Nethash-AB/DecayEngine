var DecayExport = function(root)
{
	var output = "";
	output += "<?xml version=\"1.0\"?>\n";
	output += "<resources xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n";

	output += "    <texture id=\"" + root.texture.trimmedName + "_d\">\n";
	output += "        <properties>\n";
	output += "            <type>diffuse</type>\n";
	output += "            <source>./" + root.texture.fullName.split(".")[0] + ".dds</source>\n";
	output += "        </properties>\n";
	output += "    </texture>\n";

	if (root.texture.normalMapFileName) {
		output += "    <texture id=\"" + root.texture.trimmedName + "_n\">\n";
		output += "        <properties>\n";
		output += "            <type>normal</type>\n";
		output += "            <source>./" + root.texture.normalMapFileName.split(".")[0] + ".dds</source>\n";
		output += "        </properties>\n";
		output += "    </texture>\n";
	}

	output += "    <material id=\"" + root.exporterProperties.materialid + "\">\n";
	output += "        <properties>\n";
	output += "            <textures>\n";
	output += "                <diffuse>\n";
	output += "                    <resource id=\"" + root.texture.trimmedName + "_d\" />\n";
	output += "                </diffuse>\n";
	if (root.texture.normalMapFileName) {
		output += "                <normal>\n";
		output += "                    <resource id=\"" + root.texture.trimmedName + "_n\" />\n";
		output += "                </normal>\n";
	}
	output += "            </textures>\n";

	output += "            <animationFrames>\n";
	for (var i = 0; i < root.allSprites.length; i++) {
		var sprite = root.allSprites[i];
		output += "                <frame>\n";
		output += "                    <vertices>\n";
		for (var j = 0; j < sprite.vertices.length; j++) {
			var vertex = sprite.vertices[j];
			var vertexUv = sprite.verticesUV[j];

			var width = sprite.untrimmedSize.width;
			var height = sprite.untrimmedSize.height;
			var xAspectRatioCorrectionFactor = 1;
			var yAspectRatioCorrectionFactor = 1;
			if (width > height) {
				xAspectRatioCorrectionFactor *= width / height;
			} else if (width < height) {
				yAspectRatioCorrectionFactor *= width / height;
			}

			var x = ((vertex.x - (width / 2)) / width) * yAspectRatioCorrectionFactor;
			var y = ((vertex.y - (height / 2)) / height) * xAspectRatioCorrectionFactor * -1;

			var u = vertexUv.x / root.texture.size.width;
			var v = vertexUv.y / root.texture.size.height;

			output += "                        <vertex x=\"" + x + "\" y=\"" + y + "\" u=\"" + u + "\" v=\"" + v + "\" />\n";
		}
		output += "                    </vertices>\n";

		output += "                    <triangles>\n";
		for (var j = 0; j < sprite.triangleIndices.length; j += 3) {
			output += "                        <triangle v1=\"" + sprite.triangleIndices[j + 2] + "\" v2=\"" + sprite.triangleIndices[j + 1] + "\" v3=\"" + sprite.triangleIndices[j] + "\" />\n";
		}
		output += "                    </triangles>\n";
		output += "                </frame>\n";
	}
	output += "            </animationFrames>\n";
	output += "        </properties>\n";
	output += "    </material>\n";
	
	output += "</resources>";

	return output;
};
DecayExport.filterName = "DecayExport";
Library.addFilter("DecayExport");
