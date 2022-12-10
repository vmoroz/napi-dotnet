'use strict';
const assert = require('assert');
const addon = require(process.env['TEST_NODE_API_MODULE_PATH']);

assert.strictEqual(addon.add(3, 5), 8);
