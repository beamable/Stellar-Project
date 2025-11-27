#!/usr/bin/env node
const { spawn } = require('child_process');
const path = require('path');

process.env.ESLINT_USE_FLAT_CONFIG = 'true';

const binName = process.platform === 'win32' ? 'eslint.cmd' : 'eslint';
const eslintBin = path.resolve(__dirname, '..', 'node_modules', '.bin', binName);

const child = spawn(eslintBin, ['.'], {
  stdio: 'inherit',
  env: process.env,
  shell: process.platform === 'win32',
});

child.on('exit', (code) => {
  process.exit(code ?? 0);
});
