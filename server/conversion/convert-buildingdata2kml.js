var fs = require('fs');
var PD = require('pretty-data').pd;
var format = require('string-template');

var FILEPATH = __dirname + '/../../data';
var INPUTFILE = FILEPATH + '/' + 'building_data.json';
var OUTFILE = FILEPATH + '/' + 'building_data.kml';

var kmlDocTemplate = '<?xml version="1.0" encoding="UTF-8"?>\
                      <kml xmlns="http://www.opengis.net/kml/2.2">\
                      <Document>\
                      <name>ServiceCentre.geojson</name>\
                      <description>/GovHack2016-Christchurch/CCC-General-Datasets/ServiceCentre/</description>\
                        {PLACEMARKS}\
                        </Document>\
                      </kml>\
                        ';

var placeTemplate = '<Placemark>\
                          <name>{NAME}</name>\
                          <description>{DESCRIPTION}</description>\
                          <Point>\
                            <coordinates>{LANG},{LONG},{ELEVATION}</coordinates>\
                          </Point>\
                          <MetaData name="Data Type">{DATATYPE}</MetaData>\
                          <MetaData name="Data Name">{DATANAME}</MetaData>\
                      </Placemark>\
                      ';

// Read the file and send to the callback
fs.readFile(INPUTFILE, handleFile)

// Write the callback function
function handleFile(err, data) {
  if (err) throw err
  obj = JSON.parse(data)

  var places = '';
  // You can now play with your data
  obj.features.forEach(function (feature) {
    var place = format(placeTemplate, {
      'NAME': feature.properties.ServiceCe1,
      'DESCRIPTION': feature.properties.ServiceCe1,
      'LANG': feature.geometry.coordinates[0],
      'LONG': feature.geometry.coordinates[1],
      'ELEVATION': 0,
      'DATATYPE': 'Building',
      'DATANAME': 'Service Centre'
    });
    // process.stdout.write(PD.xml(place).toString()+'\n')

    places += place;
  });

  // save file
  fs.writeFileSync(OUTFILE, PD.xml(format(kmlDocTemplate, {'PLACEMARKS': places})));

  // verify saved file content
  process.stdout.write(fs.readFileSync(OUTFILE));
}
