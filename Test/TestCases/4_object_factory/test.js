'use strict';
const assert = require('assert');
const addon = require(process.env['TEST_NODE_API_MODULE_PATH']);

const obj1 = addon('hello');
const obj2 = addon('world');
assert.strictEqual(`${obj1.msg} ${obj2.msg}`, 'hello world');
