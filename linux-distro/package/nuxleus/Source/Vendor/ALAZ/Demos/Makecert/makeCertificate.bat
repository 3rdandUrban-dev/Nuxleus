makecert -pe -n "CN=ALAZ Library" -ss my -sr LocalMachine -a sha1 -sky exchange -eku 1.3.6.1.5.5.7.3.1 -in "ALAZ CA" -is MY  -ir LocalMachine -sp "Microsoft RSA SChannel Cryptographic Provider" -len 512 -sy 12 "ALAZ Library.cer"
pause
