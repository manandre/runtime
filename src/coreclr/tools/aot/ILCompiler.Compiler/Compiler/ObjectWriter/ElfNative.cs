// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace ILCompiler.ObjectWriter
{
    /// <summary>
    /// Native constants for the ELF file format.
    /// </summary>
    internal static class ElfNative
    {
        // ELF version
        public const byte EV_CURRENT = 1;

        // File type
        public const ushort ET_REL = 1;

        // File bitness
        public const byte ELFCLASS32 = 1;
        public const byte ELFCLASS64 = 2;

        // File endianness
        public const byte ELFDATA2LSB = 1;

        // Architecture
        public const ushort EM_386 = 3;
        public const ushort EM_ARM = 40;
        public const ushort EM_X86_64 = 62;
        public const ushort EM_AARCH64 = 183;

        // Section header type
        public const uint SHT_NULL = 0;
        public const uint SHT_PROGBITS = 1;
        public const uint SHT_SYMTAB = 2;
        public const uint SHT_STRTAB = 3;
        public const uint SHT_RELA = 4;
        public const uint SHT_HASH = 5;
        public const uint SHT_DYNAMIC = 6;
        public const uint SHT_NOTE = 7;
        public const uint SHT_NOBITS = 8;
        public const uint SHT_REL = 9;
        public const uint SHT_SHLIB = 10;
        public const uint SHT_DYNSYM = 11;
        public const uint SHT_INIT_ARRAY = 14;
        public const uint SHT_FINI_ARRAY = 15;
        public const uint SHT_PREINIT_ARRAY = 16;
        public const uint SHT_GROUP = 17;
        public const uint SHT_SYMTAB_SHNDX = 18;
        public const uint SHT_IA_64_UNWIND = 0x70000001;
        public const uint SHT_ARM_EXIDX = 0x70000001;
        public const uint SHT_ARM_ATTRIBUTES = 0x70000003;

        // Section header flags
        public const uint SHF_WRITE = 1;
        public const uint SHF_ALLOC = 2;
        public const uint SHF_EXECINSTR = 4;
        public const uint SHF_MERGE = 16;
        public const uint SHF_STRINGS = 32;
        public const uint SHF_INFO_LINK = 64;
        public const uint SHF_LINK_ORDER = 128;
        public const uint SHF_OS_NONCONFORMING = 256;
        public const uint SHF_GROUP = 512;
        public const uint SHF_TLS = 1024;
        public const uint SHF_COMPRESSED = 2048;

        // Section header special index numbers
        public const uint SHN_UNDEF = 0;
        public const uint SHN_LORESERVE = 65280;
        public const uint SHN_XINDEX = 65535;

        // Section group type
        public const uint GRP_COMDAT = 1;

        // Symbol type
        public const byte STT_NOTYPE = 0;
        public const byte STT_OBJECT = 1;
        public const byte STT_FUNC = 2;
        public const byte STT_SECTION = 3;
        public const byte STT_FILE = 4;
        public const byte STT_COMMON = 5;
        public const byte STT_TLS = 6;

        // Symbol visibility
        public const byte STV_DEFAULT = 0;
        public const byte STV_INTERNAL = 1;
        public const byte STV_HIDDEN = 2;
        public const byte STV_PROTECTED = 3;

        // Symbol binding
        public const byte STB_LOCAL = 0;
        public const byte STB_GLOBAL = 1;
        public const byte STB_WEAK = 2;

        // Relocations (x86)
        public const uint R_386_NONE = 0;
        public const uint R_386_32 = 1;
        public const uint R_386_PC32 = 2;
        public const uint R_386_GOT32 = 3;
        public const uint R_386_PLT32 = 4;
        public const uint R_386_COPY = 5;
        public const uint R_386_GLOB_DAT = 6;
        public const uint R_386_JMP_SLOT = 7;
        public const uint R_386_RELATIVE = 8;
        public const uint R_386_GOTOFF = 9;
        public const uint R_386_GOTPC = 10;
        public const uint R_386_32PLT = 11;
        public const uint R_386_TLS_TPOFF = 14;
        public const uint R_386_TLS_IE = 15;
        public const uint R_386_TLS_GOTIE = 16;
        public const uint R_386_TLS_LE = 17;
        public const uint R_386_TLS_GD = 18;
        public const uint R_386_TLS_LDM = 19;
        public const uint R_386_16 = 20;
        public const uint R_386_PC16 = 21;
        public const uint R_386_8 = 22;
        public const uint R_386_PC8 = 23;
        public const uint R_386_TLS_GD_32 = 24;
        public const uint R_386_TLS_GD_PUSH = 25;
        public const uint R_386_TLS_GD_CALL = 26;
        public const uint R_386_TLS_GD_POP = 27;
        public const uint R_386_TLS_LDM_32 = 28;
        public const uint R_386_TLS_LDM_PUSH = 29;
        public const uint R_386_TLS_LDM_CALL = 30;
        public const uint R_386_TLS_LDM_POP = 31;
        public const uint R_386_TLS_LDO_32 = 32;
        public const uint R_386_TLS_IE_32 = 33;
        public const uint R_386_TLS_LE_32 = 34;
        public const uint R_386_TLS_DTPMOD32 = 35;
        public const uint R_386_TLS_DTPOFF32 = 36;
        public const uint R_386_TLS_TPOFF32 = 37;
        public const uint R_386_SIZE32 = 38;
        public const uint R_386_TLS_GOTDESC = 39;
        public const uint R_386_TLS_DESC_CALL = 40;
        public const uint R_386_TLS_DESC = 41;
        public const uint R_386_IRELATIVE = 42;

        // Relocations (x64)
        public const uint R_X86_64_NONE = 0;
        public const uint R_X86_64_64 = 1;
        public const uint R_X86_64_PC32 = 2;
        public const uint R_X86_64_GOT32 = 3;
        public const uint R_X86_64_PLT32 = 4;
        public const uint R_X86_64_COPY = 5;
        public const uint R_X86_64_GLOB_DAT = 6;
        public const uint R_X86_64_JUMP_SLOT = 7;
        public const uint R_X86_64_RELATIVE = 8;
        public const uint R_X86_64_GOTPCREL = 9;
        public const uint R_X86_64_32 = 10;
        public const uint R_X86_64_32S = 11;
        public const uint R_X86_64_16 = 12;
        public const uint R_X86_64_PC16 = 13;
        public const uint R_X86_64_8 = 14;
        public const uint R_X86_64_PC8 = 15;
        public const uint R_X86_64_DTPMOD64 = 16;
        public const uint R_X86_64_DTPOFF64 = 17;
        public const uint R_X86_64_TPOFF64 = 18;
        public const uint R_X86_64_TLSGD = 19;
        public const uint R_X86_64_TLSLD = 20;
        public const uint R_X86_64_DTPOFF32 = 21;
        public const uint R_X86_64_GOTTPOFF = 22;
        public const uint R_X86_64_TPOFF32 = 23;
        public const uint R_X86_64_PC64 = 24;
        public const uint R_X86_64_GOTOFF64 = 25;
        public const uint R_X86_64_GOTPC32 = 26;
        public const uint R_X86_64_GOT64 = 27;
        public const uint R_X86_64_GOTPCREL64 = 28;
        public const uint R_X86_64_GOTPC64 = 29;
        public const uint R_X86_64_GOTPLT64 = 30;
        public const uint R_X86_64_PLTOFF64 = 31;
        public const uint R_X86_64_SIZE32 = 32;
        public const uint R_X86_64_SIZE64 = 33;
        public const uint R_X86_64_GOTPC32_TLSDESC = 34;
        public const uint R_X86_64_TLSDESC_CALL = 35;
        public const uint R_X86_64_TLSDESC = 36;
        public const uint R_X86_64_IRELATIVE = 37;
        public const uint R_X86_64_RELATIVE64 = 38;

        // Relocations (arm32)
        public const uint R_ARM_NONE = 0;
        public const uint R_ARM_PC24 = 1;
        public const uint R_ARM_ABS32 = 2;
        public const uint R_ARM_REL32 = 3;
        public const uint R_ARM_PC13 = 4;
        public const uint R_ARM_ABS16 = 5;
        public const uint R_ARM_ABS12 = 6;
        public const uint R_ARM_THM_ABS5 = 7;
        public const uint R_ARM_ABS8 = 8;
        public const uint R_ARM_SBREL32 = 9;
        public const uint R_ARM_THM_CALL = 10;
        public const uint R_ARM_THM_PC8 = 11;
        public const uint R_ARM_AMP_VCALL9 = 12;
        public const uint R_ARM_SWI24 = 13;
        public const uint R_ARM_TLS_DESC = 13;
        public const uint R_ARM_THM_SWI8 = 14;
        public const uint R_ARM_XPC25 = 15;
        public const uint R_ARM_THM_XPC22 = 16;
        public const uint R_ARM_TLS_DTPMOD32 = 17;
        public const uint R_ARM_TLS_DTPOFF32 = 18;
        public const uint R_ARM_TLS_TPOFF32 = 19;
        public const uint R_ARM_COPY = 20;
        public const uint R_ARM_GLOB_DAT = 21;
        public const uint R_ARM_JUMP_SLOT = 22;
        public const uint R_ARM_RELATIVE = 23;
        public const uint R_ARM_GOTOFF = 24;
        public const uint R_ARM_GOTPC = 25;
        public const uint R_ARM_GOT32 = 26;
        public const uint R_ARM_PLT32 = 27;
        public const uint R_ARM_CALL = 28;
        public const uint R_ARM_JUMP24 = 29;
        public const uint R_ARM_THM_JUMP24 = 30;
        public const uint R_ARM_BASE_ABS = 31;
        public const uint R_ARM_ALU_PCREL_7_0 = 32;
        public const uint R_ARM_ALU_PCREL_15_8 = 33;
        public const uint R_ARM_ALU_PCREL_23_15 = 34;
        public const uint R_ARM_LDR_SBREL_11_0 = 35;
        public const uint R_ARM_ALU_SBREL_19_12 = 36;
        public const uint R_ARM_ALU_SBREL_27_20 = 37;
        public const uint R_ARM_TARGET1 = 38;
        public const uint R_ARM_SBREL31 = 39;
        public const uint R_ARM_V4BX = 40;
        public const uint R_ARM_TARGET2 = 41;
        public const uint R_ARM_PREL31 = 42;
        public const uint R_ARM_MOVW_ABS_NC = 43;
        public const uint R_ARM_MOVT_ABS = 44;
        public const uint R_ARM_MOVW_PREL_NC = 45;
        public const uint R_ARM_MOVT_PREL = 46;
        public const uint R_ARM_THM_MOVW_ABS_NC = 47;
        public const uint R_ARM_THM_MOVT_ABS = 48;
        public const uint R_ARM_THM_MOVW_PREL_NC = 49;
        public const uint R_ARM_THM_MOVT_PREL = 50;
        public const uint R_ARM_THM_JUMP19 = 51;
        public const uint R_ARM_THM_JUMP6 = 52;
        public const uint R_ARM_THM_ALU_PREL_11_0 = 53;
        public const uint R_ARM_THM_PC12 = 54;
        public const uint R_ARM_ABS32_NOI = 55;
        public const uint R_ARM_REL32_NOI = 56;
        public const uint R_ARM_ALU_PC_G0_NC = 57;
        public const uint R_ARM_ALU_PC_G0 = 58;
        public const uint R_ARM_ALU_PC_G1_NC = 59;
        public const uint R_ARM_ALU_PC_G1 = 60;
        public const uint R_ARM_ALU_PC_G2 = 61;
        public const uint R_ARM_LDR_PC_G1 = 62;
        public const uint R_ARM_LDR_PC_G2 = 63;
        public const uint R_ARM_LDRS_PC_G0 = 64;
        public const uint R_ARM_LDRS_PC_G1 = 65;
        public const uint R_ARM_LDRS_PC_G2 = 66;
        public const uint R_ARM_LDC_PC_G0 = 67;
        public const uint R_ARM_LDC_PC_G1 = 68;
        public const uint R_ARM_LDC_PC_G2 = 69;
        public const uint R_ARM_ALU_SB_G0_NC = 70;
        public const uint R_ARM_ALU_SB_G0 = 71;
        public const uint R_ARM_ALU_SB_G1_NC = 72;
        public const uint R_ARM_ALU_SB_G1 = 73;
        public const uint R_ARM_ALU_SB_G2 = 74;
        public const uint R_ARM_LDR_SB_G0 = 75;
        public const uint R_ARM_LDR_SB_G1 = 76;
        public const uint R_ARM_LDR_SB_G2 = 77;
        public const uint R_ARM_LDRS_SB_G0 = 78;
        public const uint R_ARM_LDRS_SB_G1 = 79;
        public const uint R_ARM_LDRS_SB_G2 = 80;
        public const uint R_ARM_LDC_SB_G0 = 81;
        public const uint R_ARM_LDC_SB_G1 = 82;
        public const uint R_ARM_LDC_SB_G2 = 83;
        public const uint R_ARM_MOVW_BREL_NC = 84;
        public const uint R_ARM_MOVT_BREL = 85;
        public const uint R_ARM_MOVW_BREL = 86;
        public const uint R_ARM_THM_MOVW_BREL_NC = 87;
        public const uint R_ARM_THM_MOVT_BREL = 88;
        public const uint R_ARM_THM_MOVW_BREL = 89;
        public const uint R_ARM_TLS_GOTDESC = 90;
        public const uint R_ARM_TLS_CALL = 91;
        public const uint R_ARM_TLS_DESCSEQ = 92;
        public const uint R_ARM_THM_TLS_CALL = 93;
        public const uint R_ARM_PLT32_ABS = 94;
        public const uint R_ARM_GOT_ABS = 95;
        public const uint R_ARM_GOT_PREL = 96;
        public const uint R_ARM_GOT_BREL12 = 97;
        public const uint R_ARM_GOTOFF12 = 98;
        public const uint R_ARM_GOTRELAX = 99;
        public const uint R_ARM_GNU_VTENTRY = 100;
        public const uint R_ARM_GNU_VTINHERIT = 101;
        public const uint R_ARM_THM_PC11 = 102;
        public const uint R_ARM_THM_PC9 = 103;
        public const uint R_ARM_TLS_GD32 = 104;
        public const uint R_ARM_TLS_LDM32 = 105;
        public const uint R_ARM_TLS_LDO32 = 106;
        public const uint R_ARM_TLS_IE32 = 107;
        public const uint R_ARM_TLS_LE32 = 108;
        public const uint R_ARM_TLS_LDO12 = 109;
        public const uint R_ARM_TLS_LE12 = 110;
        public const uint R_ARM_TLS_IE12GP = 111;
        public const uint R_ARM_ME_TOO = 128;
        public const uint R_ARM_THM_TLS_DESCSEQ = 129;
        public const uint R_ARM_THM_TLS_DESCSEQ16 = 129;
        public const uint R_ARM_THM_TLS_DESCSEQ32 = 130;
        public const uint R_ARM_THM_GOT_BREL12 = 131;
        public const uint R_ARM_IRELATIVE = 160;
        public const uint R_ARM_RXPC25 = 249;
        public const uint R_ARM_RSBREL32 = 250;
        public const uint R_ARM_THM_RPC22 = 251;
        public const uint R_ARM_RREL32 = 252;
        public const uint R_ARM_RABS22 = 253;
        public const uint R_ARM_RPC24 = 254;
        public const uint R_ARM_RBASE = 255;

        // Relocations (arm64)
        public const uint R_AARCH64_NONE = 0;
        public const uint R_AARCH64_P32_ABS32 = 1;
        public const uint R_AARCH64_P32_COPY = 180;
        public const uint R_AARCH64_P32_GLOB_DAT = 181;
        public const uint R_AARCH64_P32_JUMP_SLOT = 182;
        public const uint R_AARCH64_P32_RELATIVE = 183;
        public const uint R_AARCH64_P32_TLS_DTPMOD = 184;
        public const uint R_AARCH64_P32_TLS_DTPREL = 185;
        public const uint R_AARCH64_P32_TLS_TPREL = 186;
        public const uint R_AARCH64_P32_TLSDESC = 187;
        public const uint R_AARCH64_P32_IRELATIVE = 188;
        public const uint R_AARCH64_ABS64 = 257;
        public const uint R_AARCH64_ABS32 = 258;
        public const uint R_AARCH64_ABS16 = 259;
        public const uint R_AARCH64_PREL64 = 260;
        public const uint R_AARCH64_PREL32 = 261;
        public const uint R_AARCH64_PREL16 = 262;
        public const uint R_AARCH64_MOVW_UABS_G0 = 263;
        public const uint R_AARCH64_MOVW_UABS_G0_NC = 264;
        public const uint R_AARCH64_MOVW_UABS_G1 = 265;
        public const uint R_AARCH64_MOVW_UABS_G1_NC = 266;
        public const uint R_AARCH64_MOVW_UABS_G2 = 267;
        public const uint R_AARCH64_MOVW_UABS_G2_NC = 268;
        public const uint R_AARCH64_MOVW_UABS_G3 = 269;
        public const uint R_AARCH64_MOVW_SABS_G0 = 270;
        public const uint R_AARCH64_MOVW_SABS_G1 = 271;
        public const uint R_AARCH64_MOVW_SABS_G2 = 272;
        public const uint R_AARCH64_LD_PREL_LO19 = 273;
        public const uint R_AARCH64_ADR_PREL_LO21 = 274;
        public const uint R_AARCH64_ADR_PREL_PG_HI21 = 275;
        public const uint R_AARCH64_ADR_PREL_PG_HI21_NC = 276;
        public const uint R_AARCH64_ADD_ABS_LO12_NC = 277;
        public const uint R_AARCH64_LDST8_ABS_LO12_NC = 278;
        public const uint R_AARCH64_TSTBR14 = 279;
        public const uint R_AARCH64_CONDBR19 = 280;
        public const uint R_AARCH64_JUMP26 = 282;
        public const uint R_AARCH64_CALL26 = 283;
        public const uint R_AARCH64_LDST16_ABS_LO12_NC = 284;
        public const uint R_AARCH64_LDST32_ABS_LO12_NC = 285;
        public const uint R_AARCH64_LDST64_ABS_LO12_NC = 286;
        public const uint R_AARCH64_MOVW_PREL_G0 = 287;
        public const uint R_AARCH64_MOVW_PREL_G0_NC = 288;
        public const uint R_AARCH64_MOVW_PREL_G1 = 289;
        public const uint R_AARCH64_MOVW_PREL_G1_NC = 290;
        public const uint R_AARCH64_MOVW_PREL_G2 = 291;
        public const uint R_AARCH64_MOVW_PREL_G2_NC = 292;
        public const uint R_AARCH64_MOVW_PREL_G3 = 293;
        public const uint R_AARCH64_LDST128_ABS_LO12_NC = 299;
        public const uint R_AARCH64_MOVW_GOTOFF_G0 = 300;
        public const uint R_AARCH64_MOVW_GOTOFF_G0_NC = 301;
        public const uint R_AARCH64_MOVW_GOTOFF_G1 = 302;
        public const uint R_AARCH64_MOVW_GOTOFF_G1_NC = 303;
        public const uint R_AARCH64_MOVW_GOTOFF_G2 = 304;
        public const uint R_AARCH64_MOVW_GOTOFF_G2_NC = 305;
        public const uint R_AARCH64_MOVW_GOTOFF_G3 = 306;
        public const uint R_AARCH64_GOTREL64 = 307;
        public const uint R_AARCH64_GOTREL32 = 308;
        public const uint R_AARCH64_GOT_LD_PREL19 = 309;
        public const uint R_AARCH64_LD64_GOTOFF_LO15 = 310;
        public const uint R_AARCH64_ADR_GOT_PAGE = 311;
        public const uint R_AARCH64_LD64_GOT_LO12_NC = 312;
        public const uint R_AARCH64_LD64_GOTPAGE_LO15 = 313;
        public const uint R_AARCH64_TLSGD_ADR_PREL21 = 512;
        public const uint R_AARCH64_TLSGD_ADR_PAGE21 = 513;
        public const uint R_AARCH64_TLSGD_ADD_LO12_NC = 514;
        public const uint R_AARCH64_TLSGD_MOVW_G1 = 515;
        public const uint R_AARCH64_TLSGD_MOVW_G0_NC = 516;
        public const uint R_AARCH64_TLSLD_ADR_PREL21 = 517;
        public const uint R_AARCH64_TLSLD_ADR_PAGE21 = 518;
        public const uint R_AARCH64_TLSLD_ADD_LO12_NC = 519;
        public const uint R_AARCH64_TLSLD_MOVW_G1 = 520;
        public const uint R_AARCH64_TLSLD_MOVW_G0_NC = 521;
        public const uint R_AARCH64_TLSLD_LD_PREL19 = 522;
        public const uint R_AARCH64_TLSLD_MOVW_DTPREL_G2 = 523;
        public const uint R_AARCH64_TLSLD_MOVW_DTPREL_G1 = 524;
        public const uint R_AARCH64_TLSLD_MOVW_DTPREL_G1_NC = 525;
        public const uint R_AARCH64_TLSLD_MOVW_DTPREL_G0 = 526;
        public const uint R_AARCH64_TLSLD_MOVW_DTPREL_G0_NC = 527;
        public const uint R_AARCH64_TLSLD_ADD_DTPREL_HI12 = 528;
        public const uint R_AARCH64_TLSLD_ADD_DTPREL_LO12 = 529;
        public const uint R_AARCH64_TLSLD_ADD_DTPREL_LO12_NC = 530;
        public const uint R_AARCH64_TLSLD_LDST8_DTPREL_LO12 = 531;
        public const uint R_AARCH64_TLSLD_LDST8_DTPREL_LO12_NC = 532;
        public const uint R_AARCH64_TLSLD_LDST16_DTPREL_LO12 = 533;
        public const uint R_AARCH64_TLSLD_LDST16_DTPREL_LO12_NC = 534;
        public const uint R_AARCH64_TLSLD_LDST32_DTPREL_LO12 = 535;
        public const uint R_AARCH64_TLSLD_LDST32_DTPREL_LO12_NC = 536;
        public const uint R_AARCH64_TLSLD_LDST64_DTPREL_LO12 = 537;
        public const uint R_AARCH64_TLSLD_LDST64_DTPREL_LO12_NC = 538;
        public const uint R_AARCH64_TLSIE_MOVW_GOTTPREL_G1 = 539;
        public const uint R_AARCH64_TLSIE_MOVW_GOTTPREL_G0_NC = 540;
        public const uint R_AARCH64_TLSIE_ADR_GOTTPREL_PAGE21 = 541;
        public const uint R_AARCH64_TLSIE_LD64_GOTTPREL_LO12_NC = 542;
        public const uint R_AARCH64_TLSIE_LD_GOTTPREL_PREL19 = 543;
        public const uint R_AARCH64_TLSLE_MOVW_TPREL_G2 = 544;
        public const uint R_AARCH64_TLSLE_MOVW_TPREL_G1 = 545;
        public const uint R_AARCH64_TLSLE_MOVW_TPREL_G1_NC = 546;
        public const uint R_AARCH64_TLSLE_MOVW_TPREL_G0 = 547;
        public const uint R_AARCH64_TLSLE_MOVW_TPREL_G0_NC = 548;
        public const uint R_AARCH64_TLSLE_ADD_TPREL_HI12 = 549;
        public const uint R_AARCH64_TLSLE_ADD_TPREL_LO12 = 550;
        public const uint R_AARCH64_TLSLE_ADD_TPREL_LO12_NC = 551;
        public const uint R_AARCH64_TLSLE_LDST8_TPREL_LO12 = 552;
        public const uint R_AARCH64_TLSLE_LDST8_TPREL_LO12_NC = 553;
        public const uint R_AARCH64_TLSLE_LDST16_TPREL_LO12 = 554;
        public const uint R_AARCH64_TLSLE_LDST16_TPREL_LO12_NC = 555;
        public const uint R_AARCH64_TLSLE_LDST32_TPREL_LO12 = 556;
        public const uint R_AARCH64_TLSLE_LDST32_TPREL_LO12_NC = 557;
        public const uint R_AARCH64_TLSLE_LDST64_TPREL_LO12 = 558;
        public const uint R_AARCH64_TLSLE_LDST64_TPREL_LO12_NC = 559;
        public const uint R_AARCH64_TLSDESC_LD_PREL19 = 560;
        public const uint R_AARCH64_TLSDESC_ADR_PREL21 = 561;
        public const uint R_AARCH64_TLSDESC_ADR_PAGE21 = 562;
        public const uint R_AARCH64_TLSDESC_LD64_LO12 = 563;
        public const uint R_AARCH64_TLSDESC_ADD_LO12 = 564;
        public const uint R_AARCH64_TLSDESC_OFF_G1 = 565;
        public const uint R_AARCH64_TLSDESC_OFF_G0_NC = 566;
        public const uint R_AARCH64_TLSDESC_LDR = 567;
        public const uint R_AARCH64_TLSDESC_ADD = 568;
        public const uint R_AARCH64_TLSDESC_CALL = 569;
        public const uint R_AARCH64_TLSLE_LDST128_TPREL_LO12 = 570;
        public const uint R_AARCH64_TLSLE_LDST128_TPREL_LO12_NC = 571;
        public const uint R_AARCH64_TLSLD_LDST128_DTPREL_LO12 = 572;
        public const uint R_AARCH64_TLSLD_LDST128_DTPREL_LO12_NC = 573;
        public const uint R_AARCH64_COPY = 1024;
        public const uint R_AARCH64_GLOB_DAT = 1025;
        public const uint R_AARCH64_JUMP_SLOT = 1026;
        public const uint R_AARCH64_RELATIVE = 1027;
        public const uint R_AARCH64_TLS_DTPMOD = 1028;
        public const uint R_AARCH64_TLS_DTPREL = 1029;
        public const uint R_AARCH64_TLS_TPREL = 1030;
        public const uint R_AARCH64_TLSDESC = 1031;
        public const uint R_AARCH64_IRELATIVE = 1032;
    }
}
