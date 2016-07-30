var fs = require('fs');
var csv = require('csv');
var PD = require('pretty-data').pd;
var format = require('string-template');

var FILEPATH = __dirname+'/../..';
var INPUTFILE = FILEPATH+'/'+'20160214_001343.CSV';
var OUTFILE = FILEPATH+'/'+'20160214_001343.kml';

var kmlDocTemplate = '<?xml version="1.0" encoding="UTF-8"?>\
  <kml xmlns="http://www.opengis.net/kml/2.2">\
  <Document>\
  <name>20160214_001343.CSV</name>\
<description>ftp://ftp.geonet.org.nz/strong/processed/Proc/2016/</description>\
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
  <MetaData name="Earthquake Date (UT)">{DATE}</MetaData>\
  <MetaData name="Time (UT)">{TIME}</MetaData>\
<MetaData name="Magnitude">{MAGNITUDE}</MetaData>\
  <MetaData name="PGA Vertical (%g)">{PGA_V}</MetaData>\
  <MetaData name="PGA Horiz_1 (%g)">{PGAH1}</MetaData>\
  <MetaData name="PGA Horiz_2 (%g)">{PAGH2}</MetaData>\
  </Placemark>\
';


//var generator = csv.generate({seed: 1, columns: 2, length: 20});
var reader = fs.createReadStream(INPUTFILE);
//
var parser = csv.parse({delimiter: ','});

var transformer = csv.transform(function(data){
  return data.map(function(value){
    return value;
  });
});

reader.on('readable', function(){
  while(data = reader.read()){
    parser.write(data);
  }
});

//
// 0	Earthquake Date (UT)	14/02/16
// 1	Time (UT)	00:13:43
// 2	ID	2016p118944
// 3	Mag Type	M
// 4	Magnitude	5.7
// 5	Depth (km)	8
// 6	R?
// 7	Epic. Dist.(km)	2
// 8	PGA Vertical (mm/s/s)	3557.4
// 9	PGA Horiz_1 (mm/s/s)	1787.9
// 10	PGA Horiz_2 (mm/s/s)	2079.9
// 11	PGA Vertical (%g)	36.2756
// 12	PGA Horiz_1 (%g)	18.2319
// 13	PGA Horiz_2 (%g)	21.2096
// 14	Accelerogram ID	20160214_001345_NBLC_20
// 15	Site Code	NBLC
// 16	Name	New Brighton Library
// 17	Site Latitude	-43.50685883
// 18	Site Longitude	172.7313538
// 19	Site Elevation (m)	0

parser.on('readable', function(){
  var places = '';
  while(data = parser.read()){
    // transformer.write(data);
    var place = format(placeTemplate, {
      'NAME': data[15],
      'DESCRIPTION': data[16],
      'LANG': data[18],
      'LONG': data[17],
      'ELEVATION': data[19],
      'DATATYPE': 'Sensor',
      'DATE': data[0],
      'TIME': data[1],
      'MAGNITUDE': data[4],
      'PGA_V': data[11],
      'PGAH1': data[12],
      'PAGH2': data[13]
    });
    places += place;

    // process.stdout.write(PD.xml(place)+'\n')
  }
  // save file
  fs.writeFileSync(OUTFILE, format(kmlDocTemplate, {'PLACEMARKS':places}));

  // verify saved file content
  process.stdout.write(PD.xml(fs.readFileSync(OUTFILE).toString()));
});

// var stringifier = csv.stringify();

// transformer.on('readable', function(){
//   while(data = transformer.read()){
//     stringifier.write(data);
//   }
// });

// stringifier.on('readable', function(){
//   while(data = stringifier.read()){
//     process.stdout.write(data)
//     // fill kml placemark
//     var place = format(placeTemplate, {
//       'NAME': data[15],
//       'DESCRIPTION': data[16],
//       'LANG': data[18],
//       'LONG': data[17],
//       'ELEVATION': data[19],
//       'DATATYPE': 'Sensor',
//       'DATE': data[0],
//       'TIME': data[1],
//       'MAGNITUDE': data[4],
//       'PGA_V': data[11],
//       'PGAH1': data[12],
//       'PAGH2': data[13]
//     });
//     process.stdout.write(PD.xml(place)+'\n')
//   }
// });

