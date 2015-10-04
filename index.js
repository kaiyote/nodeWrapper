var NativeAdapter = require('edge').func(__dirname + '\\NodeWrapper\\bin\\Release\\NodeWrapper.dll');
adapter = null;

NativeAdapter({path: 'some repo'}, function(error, native) {
  if (error) console.error(error);

  adapter = native;
});

while (1) {}
