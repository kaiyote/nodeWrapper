var NativeAdapter = require('edge').func(__dirname + '\\NodeWrapper\\bin\\Debug\\NodeWrapper.dll');
adapter = null;

NativeAdapter({path: 'C:\\RelayClearance'}, function(error, native) {
  if (error) console.error(error);

  adapter = native;
});

while (1) {}
