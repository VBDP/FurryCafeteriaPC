// ============================================================================
// iRoleSystem v5.6 - Editor Language Hub
// Author: ishiruhii
// Description: Traducciones centralizadas para todos los Inspectors/Editores.
//              Lee el idioma activo desde iRoleSystemCore en la escena.
//              ES / EN / 日本語 / 中文
// ============================================================================

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace ishiruhii.iRoleSystem.Editor
{
    // =========================================================================
    // IDs de strings del editor (no confundir con iLang, que es runtime)
    // =========================================================================
    public static class EL
    {
        // ── Secciones comunes ──────────────────────────────────────────────
        public const int SECTION_SYSTEM_REFS     = 0;
        public const int SECTION_CONFIG          = 1;
        public const int SECTION_EFFECTS         = 2;
        public const int SECTION_ROLES           = 3;
        public const int SECTION_DEBUG           = 4;
        public const int SECTION_REFERENCES      = 5;
        public const int SECTION_AUDIO           = 6;
        public const int SECTION_UI              = 7;
        public const int SECTION_OPTIONS         = 8;
        public const int SECTION_PERF            = 9;
        public const int SECTION_PRISON_CONFIG   = 10;
        public const int SECTION_CHECK_CONFIG    = 11;
        public const int SECTION_ZONE_CONFIG     = 12;
        public const int SECTION_POS_SCALE       = 13;
        public const int SECTION_BILLBOARD       = 14;
        public const int SECTION_ANIMATION       = 15;
        public const int SECTION_PLAYER_LIST     = 16;
        public const int SECTION_ADMIN_LIST      = 17;
        public const int SECTION_GLOBAL_CFG      = 18;
        public const int SECTION_INPUT           = 19;
        public const int SECTION_CANVAS_POS      = 20;
        public const int SECTION_WHITELIST       = 21;
        public const int SECTION_ROLE_CHECK      = 22;

        // ── Campos de campo comunes ────────────────────────────────────────
        public const int FIELD_ROLE_DATABASE     = 30;
        public const int FIELD_PLAYER_MANAGER    = 31;
        public const int FIELD_DEFAULT_ROLE      = 32;
        public const int FIELD_MAX_PLAYERS       = 33;
        public const int FIELD_PRISON_AREA       = 34;
        public const int FIELD_TELEPORT_POINT    = 35;
        public const int FIELD_POS_INTERVAL      = 36;
        public const int FIELD_DYN_ROLE_CHECK    = 37;
        public const int FIELD_ROLE_INTERVAL     = 38;
        public const int FIELD_TELEPORT_SOUND    = 39;
        public const int FIELD_TELEPORT_FX       = 40;
        public const int FIELD_EJECTION_POINT    = 41;
        public const int FIELD_EJECTION_MSG      = 42;
        public const int FIELD_EJECTION_SOUND    = 43;
        public const int FIELD_ASSIGN_SOUND      = 44;
        public const int FIELD_ROLE_TO_ASSIGN    = 45;
        public const int FIELD_ONLY_IF_NO_ROLE   = 46;
        public const int FIELD_ROLE_TO_GRANT     = 47;
        public const int FIELD_AUTH_USERS        = 48;
        public const int FIELD_HIDE_UNAUTH       = 49;
        public const int FIELD_GRANT_SOUND       = 50;
        public const int FIELD_CHECK_INTERVAL    = 51;
        public const int FIELD_DEFAULT_VIS       = 52;
        public const int FIELD_VIS_WITH_PERM     = 53;
        public const int FIELD_VIS_WITHOUT_PERM  = 54;
        public const int FIELD_HEIGHT_OFFSET     = 55;
        public const int FIELD_DISPLAY_SCALE     = 56;
        public const int FIELD_BILLBOARD         = 57;
        public const int FIELD_BILLBOARD_Y       = 58;
        public const int FIELD_ANIM_CLIP         = 59;
        public const int FIELD_LOOP_ANIM         = 60;
        public const int FIELD_UPDATE_FRAMES     = 61;
        public const int FIELD_MAX_CAPACITY      = 62;
        public const int FIELD_DISPLAY_CONFIGS   = 63;
        public const int FIELD_DISPLAY_PREFABS   = 64;
        public const int FIELD_ASSIGN_ON_JOIN    = 65;
        public const int FIELD_ASSIGN_DELAY      = 66;
        public const int FIELD_DEBUG_MODE        = 67;
        public const int FIELD_FEEDBACK_SOUND    = 68;
        public const int FIELD_FEEDBACK_TEXT     = 69;
        public const int FIELD_MAX_SLOTS         = 70;
        public const int FIELD_FALLBACK_PREFAB   = 71;
        public const int FIELD_CANVAS            = 72;
        public const int FIELD_BTN_CONTAINER     = 73;
        public const int FIELD_MENU_OPEN_SOUND   = 74;
        public const int FIELD_ACCESS_DENIED_SND = 75;
        public const int FIELD_ROLE_INTERVAL_TXT = 76;
        public const int FIELD_ROLE_DISPLAY_TXT  = 77;
        public const int FIELD_MAX_BUTTONS       = 78;
        public const int FIELD_ENABLE_KEY_TOGGLE = 79;
        public const int FIELD_PC_KEY            = 80;
        public const int FIELD_QUEST_BUTTON      = 81;
        public const int FIELD_POS_IN_FRONT      = 82;
        public const int FIELD_CANVAS_DIST       = 83;
        public const int FIELD_HEIGHT_OFFSET2    = 84;
        public const int FIELD_EXTRA_ROT         = 85;
        public const int FIELD_CANVAS_SCALE      = 86;
        public const int FIELD_OPEN_SOUND        = 87;
        public const int FIELD_CLOSE_SOUND       = 88;
        public const int FIELD_ERROR_SOUND       = 89;
        public const int FIELD_CLEAR_ON_CLOSE    = 90;
        public const int FIELD_INFO_ON_TYPE      = 91;
        public const int FIELD_INFO_INTERVAL     = 92;
        public const int FIELD_AUTO_CLOSE        = 93;
        public const int FIELD_AUTO_CLOSE_DELAY  = 94;
        public const int FIELD_TARGET_INPUT      = 95;
        public const int FIELD_PLAYER_INFO_TXT   = 96;
        public const int FIELD_CANVAS_ROOT       = 97;
        public const int FIELD_DIST_MULT         = 98;
        public const int FIELD_VERT_OFFSET       = 99;
        public const int FIELD_BASE_HEIGHT       = 100;
        public const int FIELD_BASE_SCALE        = 101;
        public const int FIELD_FACING_PLAYER     = 102;
        public const int FIELD_FLAT_ROTATION     = 103;
        public const int FIELD_COOLDOWN          = 104;
        public const int FIELD_DISPLAY_PREFAB    = 105;
        public const int FIELD_ROLE_CHECK_INT    = 106;
        public const int FIELD_ASSIGN_DELAY_S    = 107;
        public const int FIELD_NO_ROLE_OPT      = 108;

        // ── Botones ────────────────────────────────────────────────────────
        public const int BTN_ADD_ROLE            = 120;
        public const int BTN_ADD_PLAYER          = 121;
        public const int BTN_ADD_ADMIN           = 122;
        public const int BTN_ADD_USER            = 123;
        public const int BTN_ADD_SLOT            = 124;
        public const int BTN_SELECT_ALL          = 125;
        public const int BTN_DESELECT_ALL        = 126;
        public const int BTN_REFRESH_ALL         = 127;
        public const int BTN_SYNC_SIZES          = 128;
        public const int BTN_REPAIR_ARRAYS       = 129;
        public const int BTN_SYNC_FROM_NGR       = 130;
        public const int BTN_SYNC_WHITELIST      = 131;
        public const int BTN_RESET_ROT           = 132;
        public const int BTN_NO_ROT              = 133;
        public const int BTN_INSTALL             = 134;
        public const int BTN_OPEN_SETUP          = 135;
        public const int BTN_DOCS                = 136;

        // ── Helpbox / mensajes ─────────────────────────────────────────────
        public const int MSG_ASSIGN_ROLE_DB      = 150;
        public const int MSG_ASSIGN_PLAYER_MGR   = 151;
        public const int MSG_NO_ROLE_DB_SCENE    = 152;
        public const int MSG_ROLE_DB_FOR_ROLES   = 153;
        public const int MSG_CONFIGS_PREFABS_SIZE = 154;
        public const int MSG_RUNTIME_CHECK_INFO  = 155;
        public const int MSG_CHECK_DISABLED      = 156;
        public const int MSG_DISPLAY_NAME_HINT   = 157;
        public const int MSG_NO_PLAYERS_CFG      = 158;
        public const int MSG_MAX_SLOTS_WARN      = 159;
        public const int MSG_ARRAY_MISMATCH      = 160;
        public const int MSG_SLOT_EMPTY          = 161;
        public const int MSG_SLOT_MAX_REACHED    = 162;
        public const int MSG_NO_ROLES_DEFINED    = 163;
        public const int MSG_EMPTY_DROPDOWN      = 164;
        public const int MSG_VOID_SLOT           = 165;
        public const int MSG_NOTIF_HINT          = 166;
        public const int MSG_CANVAS_HINT         = 167;
        public const int MSG_INPUT_HINT          = 168;
        public const int MSG_WHITELIST_HINT      = 169;
        public const int MSG_CANVAS_POS_HINT     = 170;
        public const int MSG_NO_NGR_ASSIGNED     = 171;
        public const int MSG_NO_CANVAS_TRANSFORM = 172;
        public const int MSG_ASSIGN_ROLE_DB_CORE = 173;
        public const int MSG_NO_ADMIN_CFG        = 174;
        public const int MSG_MAX_SLOTS_ADMIN_WARN = 175;

        // ── Diálogos ───────────────────────────────────────────────────────
        public const int DLG_CHANGE_MAX_SLOTS_TITLE  = 190;
        public const int DLG_CHANGE_MAX_SLOTS_BODY   = 191;
        public const int DLG_CHANGE_MAX_SLOTS_YES    = 192;
        public const int DLG_CHANGE_MAX_SLOTS_NO     = 193;
        public const int DLG_DELETE_PLAYER_TITLE     = 194;
        public const int DLG_DELETE_PLAYER_BODY      = 195;
        public const int DLG_DELETE_BTN              = 196;
        public const int DLG_CANCEL_BTN             = 197;
        public const int DLG_DELETE_ADMIN_TITLE      = 198;
        public const int DLG_DELETE_ADMIN_BODY       = 199;
        public const int DLG_INSTALL_OK_TITLE        = 200;
        public const int DLG_INSTALL_OK_BODY         = 201;
        public const int DLG_INSTALL_OK_BTN          = 202;
        public const int DLG_NO_CHANGES_TITLE        = 203;
        public const int DLG_NO_CHANGES_BODY         = 204;
        public const int DLG_ERROR_TITLE             = 205;
        public const int DLG_ERROR_NO_PREFAB         = 206;
        public const int DLG_ERROR_NO_CONTAINER      = 207;
        public const int DLG_OK                      = 208;
        public const int DLG_NO_NGR_PLAYERS          = 209;

        // ── Etiquetas misceláneas ──────────────────────────────────────────
        public const int LABEL_SUMMARY          = 220;
        public const int LABEL_TOTAL_PLAYERS    = 221;
        public const int LABEL_TOTAL_SLOTS      = 222;
        public const int LABEL_ASSIGN_JOIN      = 223;
        public const int LABEL_DELAY            = 224;
        public const int LABEL_ASSIGNMENTS      = 225;
        public const int LABEL_SCALE_PREVIEW    = 226;
        public const int LABEL_PC_MODE          = 227;
        public const int LABEL_OCULUS_MODE      = 228;
        public const int LABEL_ORIENTATION      = 229;
        public const int LABEL_SCALE            = 230;
        public const int LABEL_POSITION         = 231;
        public const int LABEL_ROLES_DEFINIDOS  = 232;
        public const int LABEL_COLORS           = 233;
        public const int LABEL_ICONS            = 234;
        public const int LABEL_ADD_ROLE         = 235;
        public const int LABEL_NEW_ROLE         = 236;
        public const int LABEL_RUNTIME_INFO     = 237;
        public const int LABEL_RUNTIME_ONLY     = 238;
        public const int LABEL_NOTIF_SECTION    = 239;
        public const int LABEL_INTERFACES       = 240;
        public const int LABEL_SOUNDS           = 241;
        public const int LABEL_FEEDBACK_TEXTS   = 242;
        public const int LABEL_AUTO_CLOSE_SEC   = 243;
        public const int LABEL_KEY_CONTROL      = 244;
        public const int LABEL_CANVAS_ROT_SCALE = 245;
        public const int LABEL_QUEST_BUTTON     = 246;
        public const int LABEL_PC_KEY_PRESS     = 247;  // "Al pulsar [X] se abrirá"
        public const int LABEL_INPUT_NAME       = 248;
        public const int LABEL_TOTAL_USERS      = 249;
        public const int LABEL_CONTROL_TYPE     = 250;
        public const int LABEL_INSTALL_MODULES  = 251;
        public const int LABEL_SELECTED_MODULES = 252;
        public const int LABEL_NONE             = 253;
        public const int LABEL_SETUP_MODULES    = 254;
        public const int LABEL_SELECT_TO_INSTALL = 255;
        public const int LABEL_MODULE_RANKDISP  = 256;
        public const int LABEL_MODULE_RANKDISP_DESC = 257;
        public const int LABEL_MODULE_OBJECTS   = 258;
        public const int LABEL_MODULE_OBJECTS_DESC = 259;
        public const int LABEL_MODULE_PRISON    = 260;
        public const int LABEL_MODULE_PRISON_DESC = 261;
        public const int LABEL_MODULE_PZONE     = 262;
        public const int LABEL_MODULE_PZONE_DESC = 263;
        public const int LABEL_SELECT_AT_LEAST  = 264;

        // Total
        public const int COUNT = 270;
    }

    // =========================================================================
    // Motor de traducción para editores
    // =========================================================================
    public static class iEditorLang
    {
        // ── Tabla ES ──────────────────────────────────────────────────────
        private static readonly string[] s_ES = new string[EL.COUNT]
        {
            /* 000 SECTION_SYSTEM_REFS     */ "Referencias del Sistema",
            /* 001 SECTION_CONFIG          */ "Configuración",
            /* 002 SECTION_EFFECTS         */ "Efectos",
            /* 003 SECTION_ROLES           */ "Roles",
            /* 004 SECTION_DEBUG           */ "Debug",
            /* 005 SECTION_REFERENCES      */ "Referencias",
            /* 006 SECTION_AUDIO           */ "Audio",
            /* 007 SECTION_UI              */ "Interfaz de Usuario",
            /* 008 SECTION_OPTIONS         */ "Opciones",
            /* 009 SECTION_PERF            */ "Configuración de Rendimiento",
            /* 010 SECTION_PRISON_CONFIG   */ "Configuración de Cárcel",
            /* 011 SECTION_CHECK_CONFIG    */ "Configuración de Chequeo",
            /* 012 SECTION_ZONE_CONFIG     */ "Configuración de Zona",
            /* 013 SECTION_POS_SCALE       */ "Posición y Escala",
            /* 014 SECTION_BILLBOARD       */ "Billboard (Mirar a Cámara)",
            /* 015 SECTION_ANIMATION       */ "Animación (Opcional)",
            /* 016 SECTION_PLAYER_LIST     */ "CONFIGURACIÓN DE JUGADORES",
            /* 017 SECTION_ADMIN_LIST      */ "CONFIGURACIÓN DE ADMINS",
            /* 018 SECTION_GLOBAL_CFG      */ "Configuración Global",
            /* 019 SECTION_INPUT           */ "Configuración de Input",
            /* 020 SECTION_CANVAS_POS      */ "Posicionamiento del Canvas",
            /* 021 SECTION_WHITELIST       */ "Whitelist de Usuarios",
            /* 022 SECTION_ROLE_CHECK      */ "Chequeo de Roles",

            // pad 23-29
            "","","","","","","",

            /* 030 FIELD_ROLE_DATABASE     */ "Base de Datos de Roles",
            /* 031 FIELD_PLAYER_MANAGER    */ "Gestor de Roles",
            /* 032 FIELD_DEFAULT_ROLE      */ "Rol por Defecto",
            /* 033 FIELD_MAX_PLAYERS       */ "Capacidad Máxima",
            /* 034 FIELD_PRISON_AREA       */ "Área de la Cárcel",
            /* 035 FIELD_TELEPORT_POINT    */ "Punto de Teletransporte",
            /* 036 FIELD_POS_INTERVAL      */ "Intervalo de Posición (s)",
            /* 037 FIELD_DYN_ROLE_CHECK    */ "Chequeo Dinámico de Rol",
            /* 038 FIELD_ROLE_INTERVAL     */ "Intervalo de Rol (s)",
            /* 039 FIELD_TELEPORT_SOUND    */ "Sonido al Teletransportar",
            /* 040 FIELD_TELEPORT_FX       */ "Partículas al Teletransportar",
            /* 041 FIELD_EJECTION_POINT    */ "Punto de Expulsión",
            /* 042 FIELD_EJECTION_MSG      */ "Mensaje de Expulsión",
            /* 043 FIELD_EJECTION_SOUND    */ "Sonido de Expulsión",
            /* 044 FIELD_ASSIGN_SOUND      */ "Sonido al Asignar",
            /* 045 FIELD_ROLE_TO_ASSIGN    */ "Rol a Asignar",
            /* 046 FIELD_ONLY_IF_NO_ROLE   */ "Solo si No Tiene Rol",
            /* 047 FIELD_ROLE_TO_GRANT     */ "Rol a Otorgar",
            /* 048 FIELD_AUTH_USERS        */ "Usuarios Autorizados",
            /* 049 FIELD_HIDE_UNAUTH       */ "Ocultar para No Autorizados",
            /* 050 FIELD_GRANT_SOUND       */ "Sonido al Otorgar",
            /* 051 FIELD_CHECK_INTERVAL    */ "Intervalo de Chequeo (s)",
            /* 052 FIELD_DEFAULT_VIS       */ "Visibilidad por Defecto",
            /* 053 FIELD_VIS_WITH_PERM     */ "Visibles CON Permiso",
            /* 054 FIELD_VIS_WITHOUT_PERM  */ "Visibles SIN Permiso",
            /* 055 FIELD_HEIGHT_OFFSET     */ "Altura sobre Cabeza",
            /* 056 FIELD_DISPLAY_SCALE     */ "Escala",
            /* 057 FIELD_BILLBOARD         */ "Activar Billboard",
            /* 058 FIELD_BILLBOARD_Y       */ "Solo Eje Y",
            /* 059 FIELD_ANIM_CLIP         */ "Clip de Animación",
            /* 060 FIELD_LOOP_ANIM         */ "Loop",
            /* 061 FIELD_UPDATE_FRAMES     */ "Actualizar cada X Frames",
            /* 062 FIELD_MAX_CAPACITY      */ "Capacidad Máxima",
            /* 063 FIELD_DISPLAY_CONFIGS   */ "Configuraciones",
            /* 064 FIELD_DISPLAY_PREFABS   */ "Prefabs Correspondientes",
            /* 065 FIELD_ASSIGN_ON_JOIN    */ "Asignar al Entrar",
            /* 066 FIELD_ASSIGN_DELAY      */ "Delay de Asignación (s)",
            /* 067 FIELD_DEBUG_MODE        */ "Modo Debug",
            /* 068 FIELD_FEEDBACK_SOUND    */ "Sonido Rol Asignado",
            /* 069 FIELD_FEEDBACK_TEXT     */ "Texto de Feedback",
            /* 070 FIELD_MAX_SLOTS         */ "Max Slots por Jugador",
            /* 071 FIELD_FALLBACK_PREFAB   */ "Prefab de Respaldo",
            /* 072 FIELD_CANVAS            */ "Canvas de Selección",
            /* 073 FIELD_BTN_CONTAINER     */ "Contenedor de Botones",
            /* 074 FIELD_MENU_OPEN_SOUND   */ "Abrir Menú",
            /* 075 FIELD_ACCESS_DENIED_SND */ "Acceso Denegado",
            /* 076 FIELD_ROLE_INTERVAL_TXT */ "Intervalo Actualización Rol (s)",
            /* 077 FIELD_ROLE_DISPLAY_TXT  */ "Texto Rol Actual del Jugador",
            /* 078 FIELD_MAX_BUTTONS       */ "Máximo de Botones",
            /* 079 FIELD_ENABLE_KEY_TOGGLE */ "Habilitar Tecla de Toggle",
            /* 080 FIELD_PC_KEY            */ "Tecla PC",
            /* 081 FIELD_QUEST_BUTTON      */ "Botón Quest",
            /* 082 FIELD_POS_IN_FRONT      */ "Posicionar Frente al Jugador",
            /* 083 FIELD_CANVAS_DIST       */ "Distancia (metros)",
            /* 084 FIELD_HEIGHT_OFFSET2    */ "Altura (metros)",
            /* 085 FIELD_EXTRA_ROT         */ "Rotación Extra (X Y Z)",
            /* 086 FIELD_CANVAS_SCALE      */ "Escala del Canvas (X Y Z)",
            /* 087 FIELD_OPEN_SOUND        */ "Sonido de Apertura",
            /* 088 FIELD_CLOSE_SOUND       */ "Sonido de Cierre",
            /* 089 FIELD_ERROR_SOUND       */ "Sonido de Error",
            /* 090 FIELD_CLEAR_ON_CLOSE    */ "Limpiar Input al Cerrar",
            /* 091 FIELD_INFO_ON_TYPE      */ "Mostrar Info al Escribir",
            /* 092 FIELD_INFO_INTERVAL     */ "Intervalo Info (s)",
            /* 093 FIELD_AUTO_CLOSE        */ "Cerrar Tras Asignar Rol",
            /* 094 FIELD_AUTO_CLOSE_DELAY  */ "Delay de Cierre (s)",
            /* 095 FIELD_TARGET_INPUT      */ "Input Nombre Jugador",
            /* 096 FIELD_PLAYER_INFO_TXT   */ "Texto Info Jugador",
            /* 097 FIELD_CANVAS_ROOT       */ "Canvas Root",
            /* 098 FIELD_DIST_MULT         */ "Distancia (× altura avatar)",
            /* 099 FIELD_VERT_OFFSET       */ "Offset Vertical (m)",
            /* 100 FIELD_BASE_HEIGHT       */ "Altura Avatar Base (m)",
            /* 101 FIELD_BASE_SCALE        */ "Escala Base del Canvas",
            /* 102 FIELD_FACING_PLAYER     */ "Mirar al Jugador",
            /* 103 FIELD_FLAT_ROTATION     */ "Solo Plano Horizontal",
            /* 104 FIELD_COOLDOWN          */ "Cooldown (s)",
            /* 105 FIELD_DISPLAY_PREFAB    */ "Prefab del Display",
            /* 106 FIELD_ROLE_CHECK_INT    */ "Intervalo de Chequeo (segundos)",
            /* 107 FIELD_ASSIGN_DELAY_S    */ "Delay de Asignación (s)",
            /* 108 FIELD_NO_ROLE_OPT       */ "Sin Rol (-1)",

            // pad 109-119
            "","","","","","","","","","","",

            /* 120 BTN_ADD_ROLE            */ "+ Añadir Nuevo Rol",
            /* 121 BTN_ADD_PLAYER          */ "+ Añadir Jugador",
            /* 122 BTN_ADD_ADMIN           */ "+ Añadir Admin",
            /* 123 BTN_ADD_USER            */ "+ Añadir Usuario",
            /* 124 BTN_ADD_SLOT            */ "+ Añadir Slot de Rol",
            /* 125 BTN_SELECT_ALL          */ "Seleccionar Todos",
            /* 126 BTN_DESELECT_ALL        */ "Deseleccionar Todos",
            /* 127 BTN_REFRESH_ALL         */ "Refrescar Todos los Displays",
            /* 128 BTN_SYNC_SIZES          */ "Sincronizar Tamaños",
            /* 129 BTN_REPAIR_ARRAYS       */ "🔧 Reparar Arrays",
            /* 130 BTN_SYNC_FROM_NGR       */ "Auto-sync desde iNameGetRole",
            /* 131 BTN_SYNC_WHITELIST      */ "↺  Sincronizar desde iNameGetRole",
            /* 132 BTN_RESET_ROT           */ "Reset (Y=180)",
            /* 133 BTN_NO_ROT              */ "Sin rotación",
            /* 134 BTN_INSTALL             */ "INSTALAR",
            /* 135 BTN_OPEN_SETUP          */ "Abrir Setup",
            /* 136 BTN_DOCS               */ "Documentación",

            // pad 137-149
            "","","","","","","","","","","","","",

            /* 150 MSG_ASSIGN_ROLE_DB      */ "Asigna un iRoleDatabase",
            /* 151 MSG_ASSIGN_PLAYER_MGR   */ "Asigna un iPlayerRoleManager",
            /* 152 MSG_NO_ROLE_DB_SCENE    */ "No se encontró iRoleDatabase en la escena",
            /* 153 MSG_ROLE_DB_FOR_ROLES   */ "Asigna un iRoleDatabase para configurar los roles",
            /* 154 MSG_CONFIGS_PREFABS_SIZE*/ "Los arrays de Configs y Prefabs deben tener el mismo tamaño",
            /* 155 MSG_RUNTIME_CHECK_INFO  */ "El sistema verificará cambios de rol cada {0} segundos",
            /* 156 MSG_CHECK_DISABLED      */ "El chequeo de roles está desactivado (0 = desactivado)",
            /* 157 MSG_DISPLAY_NAME_HINT   */ "Lista de displayNames exactos de VRChat",
            /* 158 MSG_NO_PLAYERS_CFG      */ "No hay jugadores configurados. ¡Agrega jugadores para asignar roles automáticamente!",
            /* 159 MSG_MAX_SLOTS_WARN      */ "No cambiar después de añadir jugadores con datos!",
            /* 160 MSG_ARRAY_MISMATCH      */ "⚠️ Los arrays internos no tienen el tamaño correcto. Haz clic en 'Reparar Arrays' para corregirlo.",
            /* 161 MSG_SLOT_EMPTY          */ "Este jugador no tiene roles asignados. Añade un slot de rol.",
            /* 162 MSG_SLOT_MAX_REACHED    */ "Máximo de {0} slots alcanzado. Aumenta 'Max Slots por Jugador'.",
            /* 163 MSG_NO_ROLES_DEFINED    */ "Asigna un iRoleDatabase en el Core para ver los roles disponibles.",
            /* 164 MSG_EMPTY_DROPDOWN      */ "— Vacío —",
            /* 165 MSG_VOID_SLOT           */ "Slot {0}: (vacío)",
            /* 166 MSG_NOTIF_HINT          */ "Los behaviours listados recibirán el evento '_OnPlayerRoleChanged'",
            /* 167 MSG_CANVAS_HINT         */ "Cada configuración define un tipo de display.\nEl array de prefabs debe coincidir con el de configs.",
            /* 168 MSG_INPUT_HINT          */ "Sin Canvas Transform: el menú se abrirá pero no se reposicionará.",
            /* 169 MSG_WHITELIST_HINT      */ "Solo los jugadores de esta lista podrán abrir el menú con la tecla configurada.",
            /* 170 MSG_CANVAS_POS_HINT     */ "El canvas se posicionará frente al jugador cada vez que pulse la tecla, escalado según la altura real del avatar.",
            /* 171 MSG_NO_NGR_ASSIGNED     */ "Asigna el iNameGetRole para que funcione el trigger.",
            /* 172 MSG_NO_CANVAS_TRANSFORM */ "Sin Canvas Transform: el menú se abrirá pero no se reposicionará.",
            /* 173 MSG_ASSIGN_ROLE_DB_CORE */ "Asigna un iRoleDatabase con roles definidos",
            /* 174 MSG_NO_ADMIN_CFG        */ "No hay administradores configurados. Haz clic en '+ Añadir Admin' para empezar.",
            /* 175 MSG_MAX_SLOTS_ADMIN_WARN*/ "No cambiar con admins ya configurados.",

            // pad 176-189
            "","","","","","","","","","","","","","",

            /* 190 DLG_CHANGE_MAX_SLOTS_TITLE */ "⚠️ Cambiar Max Slots",
            /* 191 DLG_CHANGE_MAX_SLOTS_BODY  */ "Cambiar 'Max Slots por Jugador' con jugadores ya configurados BORRARÁ toda la configuración de roles.\n\n¿Continuar?",
            /* 192 DLG_CHANGE_MAX_SLOTS_YES   */ "Sí, resetear",
            /* 193 DLG_CHANGE_MAX_SLOTS_NO    */ "Cancelar",
            /* 194 DLG_DELETE_PLAYER_TITLE    */ "Eliminar Jugador",
            /* 195 DLG_DELETE_PLAYER_BODY     */ "¿Eliminar a '{0}' y toda su configuración de roles?",
            /* 196 DLG_DELETE_BTN             */ "Eliminar",
            /* 197 DLG_CANCEL_BTN             */ "Cancelar",
            /* 198 DLG_DELETE_ADMIN_TITLE     */ "Eliminar Admin",
            /* 199 DLG_DELETE_ADMIN_BODY      */ "¿Eliminar admin '{0}' y todos sus slots?",
            /* 200 DLG_INSTALL_OK_TITLE       */ "Instalación Completada",
            /* 201 DLG_INSTALL_OK_BODY        */ "Se instalaron {0} módulo(s):\n\n{1}",
            /* 202 DLG_INSTALL_OK_BTN         */ "¡Genial!",
            /* 203 DLG_NO_CHANGES_TITLE       */ "Sin Cambios",
            /* 204 DLG_NO_CHANGES_BODY        */ "No se instalaron módulos. Algunos pueden ya estar instalados o no se encontraron los prefabs.",
            /* 205 DLG_ERROR_TITLE            */ "Error",
            /* 206 DLG_ERROR_NO_PREFAB        */ "No se encontró el prefab '{0}' en la escena.\n\nAsegúrate de tener el prefab principal del sistema en la escena antes de instalar módulos.",
            /* 207 DLG_ERROR_NO_CONTAINER     */ "No se encontró el objeto '{0}' dentro de '{1}'.\n\nVerifica la estructura del prefab.",
            /* 208 DLG_OK                     */ "OK",
            /* 209 DLG_NO_NGR_PLAYERS         */ "El iNameGetRole asignado no tiene jugadores configurados.",

            // pad 210-219
            "","","","","","","","","","",

            /* 220 LABEL_SUMMARY          */ "Resumen:",
            /* 221 LABEL_TOTAL_PLAYERS    */ "• Total jugadores: {0}",
            /* 222 LABEL_TOTAL_SLOTS      */ "• Total slots de rol configurados: {0}",
            /* 223 LABEL_ASSIGN_JOIN      */ "• Asignar al entrar: {0}",
            /* 224 LABEL_DELAY            */ "• Delay: {0}s",
            /* 225 LABEL_ASSIGNMENTS      */ "Asignaciones configuradas:",
            /* 226 LABEL_SCALE_PREVIEW    */ "Previsualización de escala:",
            /* 227 LABEL_PC_MODE          */ "⌨  Tecla de Teclado",
            /* 228 LABEL_OCULUS_MODE      */ "🎮  Botón de Oculus / VR",
            /* 229 LABEL_ORIENTATION      */ "Orientación",
            /* 230 LABEL_SCALE            */ "Escala",
            /* 231 LABEL_POSITION         */ "Posición",
            /* 232 LABEL_ROLES_DEFINIDOS  */ "Roles Definidos",
            /* 233 LABEL_COLORS           */ "Colores de Roles",
            /* 234 LABEL_ICONS            */ "Iconos de Roles (Opcional)",
            /* 235 LABEL_ADD_ROLE         */ "+ Añadir Nuevo Rol",
            /* 236 LABEL_NEW_ROLE         */ "Nuevo Rol",
            /* 237 LABEL_RUNTIME_INFO     */ "Info Runtime",
            /* 238 LABEL_RUNTIME_ONLY     */ "(Datos solo visibles en Play Mode)",
            /* 239 LABEL_NOTIF_SECTION    */ "Notificaciones de Cambio de Rol",
            /* 240 LABEL_INTERFACES       */ "Interfaz de Usuario",
            /* 241 LABEL_SOUNDS           */ "Sonidos",
            /* 242 LABEL_FEEDBACK_TEXTS   */ "Textos de Feedback",
            /* 243 LABEL_AUTO_CLOSE_SEC   */ "Cierre Automático",
            /* 244 LABEL_KEY_CONTROL      */ "Tipo de Control",
            /* 245 LABEL_CANVAS_ROT_SCALE */ "Rotación y Escala",
            /* 246 LABEL_QUEST_BUTTON     */ "Botón Quest:",
            /* 247 LABEL_PC_KEY_PRESS     */ "Al pulsar [ {0} ] se abrirá el menú",
            /* 248 LABEL_INPUT_NAME       */ "InputName: {0}",
            /* 249 LABEL_TOTAL_USERS      */ "Total: {0} usuario(s)",
            /* 250 LABEL_CONTROL_TYPE     */ "Tipo de Control",
            /* 251 LABEL_INSTALL_MODULES  */ "Setup de Módulos",
            /* 252 LABEL_SELECTED_MODULES */ "Módulos seleccionados:",
            /* 253 LABEL_NONE             */ "(Ninguno)",
            /* 254 LABEL_SETUP_MODULES    */ "Setup de Módulos",
            /* 255 LABEL_SELECT_TO_INSTALL*/ "Seleccione a continuación que módulos desea instalar",
            /* 256 LABEL_MODULE_RANKDISP  */ "Módulo\niRankDisplay",
            /* 257 LABEL_MODULE_RANKDISP_DESC */ "Insignias sobre\nla cabeza",
            /* 258 LABEL_MODULE_OBJECTS   */ "Módulo\niObjects",
            /* 259 LABEL_MODULE_OBJECTS_DESC  */ "Visibilidad de\nobjetos por rol",
            /* 260 LABEL_MODULE_PRISON    */ "Módulo\niPrison",
            /* 261 LABEL_MODULE_PRISON_DESC   */ "Cárcel para\njugadores",
            /* 262 LABEL_MODULE_PZONE     */ "Módulo\niPrivateRoleZone",
            /* 263 LABEL_MODULE_PZONE_DESC    */ "Zonas privadas\npor rol",
            /* 264 LABEL_SELECT_AT_LEAST  */ "Selecciona al menos un módulo para instalar",

            // pad 265-269
            "","","","",""
        };

        // ── Tabla EN ──────────────────────────────────────────────────────
        private static readonly string[] s_EN = new string[EL.COUNT]
        {
            /* 000 */ "System References",
            /* 001 */ "Configuration",
            /* 002 */ "Effects",
            /* 003 */ "Roles",
            /* 004 */ "Debug",
            /* 005 */ "References",
            /* 006 */ "Audio",
            /* 007 */ "User Interface",
            /* 008 */ "Options",
            /* 009 */ "Performance Settings",
            /* 010 */ "Prison Configuration",
            /* 011 */ "Check Configuration",
            /* 012 */ "Zone Configuration",
            /* 013 */ "Position & Scale",
            /* 014 */ "Billboard (Face Camera)",
            /* 015 */ "Animation (Optional)",
            /* 016 */ "PLAYER CONFIGURATION",
            /* 017 */ "ADMIN CONFIGURATION",
            /* 018 */ "Global Configuration",
            /* 019 */ "Input Configuration",
            /* 020 */ "Canvas Positioning",
            /* 021 */ "User Whitelist",
            /* 022 */ "Role Check",
            "","","","","","","",
            /* 030 */ "Role Database",
            /* 031 */ "Role Manager",
            /* 032 */ "Default Role",
            /* 033 */ "Maximum Capacity",
            /* 034 */ "Prison Area",
            /* 035 */ "Teleport Point",
            /* 036 */ "Position Interval (s)",
            /* 037 */ "Dynamic Role Check",
            /* 038 */ "Role Interval (s)",
            /* 039 */ "Teleport Sound",
            /* 040 */ "Teleport Particles",
            /* 041 */ "Ejection Point",
            /* 042 */ "Ejection Message",
            /* 043 */ "Ejection Sound",
            /* 044 */ "Assign Sound",
            /* 045 */ "Role to Assign",
            /* 046 */ "Only if No Role",
            /* 047 */ "Role to Grant",
            /* 048 */ "Authorized Users",
            /* 049 */ "Hide for Unauthorized",
            /* 050 */ "Grant Sound",
            /* 051 */ "Check Interval (s)",
            /* 052 */ "Default Visibility",
            /* 053 */ "Visible WITH Permission",
            /* 054 */ "Visible WITHOUT Permission",
            /* 055 */ "Height Above Head",
            /* 056 */ "Scale",
            /* 057 */ "Enable Billboard",
            /* 058 */ "Y Axis Only",
            /* 059 */ "Animation Clip",
            /* 060 */ "Loop",
            /* 061 */ "Update every X Frames",
            /* 062 */ "Maximum Capacity",
            /* 063 */ "Configurations",
            /* 064 */ "Corresponding Prefabs",
            /* 065 */ "Assign on Join",
            /* 066 */ "Assignment Delay (s)",
            /* 067 */ "Debug Mode",
            /* 068 */ "Role Assigned Sound",
            /* 069 */ "Feedback Text",
            /* 070 */ "Max Slots per Player",
            /* 071 */ "Fallback Prefab",
            /* 072 */ "Selection Canvas",
            /* 073 */ "Button Container",
            /* 074 */ "Open Menu",
            /* 075 */ "Access Denied",
            /* 076 */ "Role Update Interval (s)",
            /* 077 */ "Current Player Role Text",
            /* 078 */ "Maximum Buttons",
            /* 079 */ "Enable Toggle Key",
            /* 080 */ "PC Key",
            /* 081 */ "Quest Button",
            /* 082 */ "Position in Front of Player",
            /* 083 */ "Distance (meters)",
            /* 084 */ "Height (meters)",
            /* 085 */ "Extra Rotation (X Y Z)",
            /* 086 */ "Canvas Scale (X Y Z)",
            /* 087 */ "Open Sound",
            /* 088 */ "Close Sound",
            /* 089 */ "Error Sound",
            /* 090 */ "Clear Input on Close",
            /* 091 */ "Show Info While Typing",
            /* 092 */ "Info Interval (s)",
            /* 093 */ "Close After Role Assign",
            /* 094 */ "Close Delay (s)",
            /* 095 */ "Player Name Input",
            /* 096 */ "Player Info Text",
            /* 097 */ "Canvas Root",
            /* 098 */ "Distance (× avatar height)",
            /* 099 */ "Vertical Offset (m)",
            /* 100 */ "Base Avatar Height (m)",
            /* 101 */ "Base Canvas Scale",
            /* 102 */ "Face Player",
            /* 103 */ "Horizontal Plane Only",
            /* 104 */ "Cooldown (s)",
            /* 105 */ "Display Prefab",
            /* 106 */ "Check Interval (seconds)",
            /* 107 */ "Assignment Delay (s)",
            /* 108 */ "No Role (-1)",
            "","","","","","","","","","","",
            /* 120 */ "+ Add New Role",
            /* 121 */ "+ Add Player",
            /* 122 */ "+ Add Admin",
            /* 123 */ "+ Add User",
            /* 124 */ "+ Add Role Slot",
            /* 125 */ "Select All",
            /* 126 */ "Deselect All",
            /* 127 */ "Refresh All Displays",
            /* 128 */ "Sync Sizes",
            /* 129 */ "🔧 Repair Arrays",
            /* 130 */ "Auto-sync from iNameGetRole",
            /* 131 */ "↺  Sync from iNameGetRole",
            /* 132 */ "Reset (Y=180)",
            /* 133 */ "No Rotation",
            /* 134 */ "INSTALL",
            /* 135 */ "Open Setup",
            /* 136 */ "Documentation",
            "","","","","","","","","","","","","",
            /* 150 */ "Assign an iRoleDatabase",
            /* 151 */ "Assign an iPlayerRoleManager",
            /* 152 */ "No iRoleDatabase found in scene",
            /* 153 */ "Assign an iRoleDatabase to configure roles",
            /* 154 */ "Configs and Prefabs arrays must be the same size",
            /* 155 */ "The system will check role changes every {0} seconds",
            /* 156 */ "Role check is disabled (0 = disabled)",
            /* 157 */ "Exact VRChat displayNames list",
            /* 158 */ "No players configured. Add players to automatically assign roles!",
            /* 159 */ "Do not change after adding players with data!",
            /* 160 */ "⚠️ Internal arrays have incorrect size. Click 'Repair Arrays' to fix it.",
            /* 161 */ "This player has no assigned roles. Add a role slot.",
            /* 162 */ "Maximum {0} slots reached. Increase 'Max Slots per Player'.",
            /* 163 */ "Assign an iRoleDatabase in Core to see available roles.",
            /* 164 */ "— Empty —",
            /* 165 */ "Slot {0}: (empty)",
            /* 166 */ "Listed behaviours will receive the '_OnPlayerRoleChanged' event",
            /* 167 */ "Each config defines a display type.\nThe prefabs array must match the configs array.",
            /* 168 */ "No Canvas Transform: the menu will open but won't reposition.",
            /* 169 */ "Only players in this list will be able to open the menu with the configured key.",
            /* 170 */ "The canvas will be positioned in front of the player each time they press the key, scaled by the avatar's real height.",
            /* 171 */ "Assign the iNameGetRole for the trigger to work.",
            /* 172 */ "No Canvas Transform: menu will open but won't reposition.",
            /* 173 */ "Assign an iRoleDatabase with defined roles",
            /* 174 */ "No admins configured. Click '+ Add Admin' to get started.",
            /* 175 */ "Do not change with admins already configured.",
            "","","","","","","","","","","","","","",
            /* 190 */ "⚠️ Change Max Slots",
            /* 191 */ "Changing 'Max Slots per Player' with already configured players WILL DELETE all role configuration.\n\nContinue?",
            /* 192 */ "Yes, reset",
            /* 193 */ "Cancel",
            /* 194 */ "Delete Player",
            /* 195 */ "Delete '{0}' and all their role configuration?",
            /* 196 */ "Delete",
            /* 197 */ "Cancel",
            /* 198 */ "Delete Admin",
            /* 199 */ "Delete admin '{0}' and all their slots?",
            /* 200 */ "Installation Complete",
            /* 201 */ "{0} module(s) installed:\n\n{1}",
            /* 202 */ "Great!",
            /* 203 */ "No Changes",
            /* 204 */ "No modules were installed. Some may already be installed or the prefabs were not found.",
            /* 205 */ "Error",
            /* 206 */ "Prefab '{0}' not found in the scene.\n\nMake sure the main system prefab is in the scene before installing modules.",
            /* 207 */ "Object '{0}' not found inside '{1}'.\n\nCheck the prefab structure.",
            /* 208 */ "OK",
            /* 209 */ "The assigned iNameGetRole has no players configured.",
            "","","","","","","","","","",
            /* 220 */ "Summary:",
            /* 221 */ "• Total players: {0}",
            /* 222 */ "• Total role slots configured: {0}",
            /* 223 */ "• Assign on join: {0}",
            /* 224 */ "• Delay: {0}s",
            /* 225 */ "Configured assignments:",
            /* 226 */ "Scale preview:",
            /* 227 */ "⌨  Keyboard Key",
            /* 228 */ "🎮  Oculus / VR Button",
            /* 229 */ "Orientation",
            /* 230 */ "Scale",
            /* 231 */ "Position",
            /* 232 */ "Defined Roles",
            /* 233 */ "Role Colors",
            /* 234 */ "Role Icons (Optional)",
            /* 235 */ "+ Add New Role",
            /* 236 */ "New Role",
            /* 237 */ "Runtime Info",
            /* 238 */ "(Data only visible in Play Mode)",
            /* 239 */ "Role Change Notifications",
            /* 240 */ "User Interface",
            /* 241 */ "Sounds",
            /* 242 */ "Feedback Texts",
            /* 243 */ "Auto Close",
            /* 244 */ "Control Type",
            /* 245 */ "Rotation & Scale",
            /* 246 */ "Quest Button:",
            /* 247 */ "Press [ {0} ] to open the menu",
            /* 248 */ "InputName: {0}",
            /* 249 */ "Total: {0} user(s)",
            /* 250 */ "Control Type",
            /* 251 */ "Module Setup",
            /* 252 */ "Selected modules:",
            /* 253 */ "(None)",
            /* 254 */ "Module Setup",
            /* 255 */ "Select the modules you want to install below",
            /* 256 */ "Module\niRankDisplay",
            /* 257 */ "Badges above\nhead",
            /* 258 */ "Module\niObjects",
            /* 259 */ "Object visibility\nby role",
            /* 260 */ "Module\niPrison",
            /* 261 */ "Prison for\nplayers",
            /* 262 */ "Module\niPrivateRoleZone",
            /* 263 */ "Private zones\nby role",
            /* 264 */ "Select at least one module to install",
            "","","","",""
        };

        // ── Tabla JP ──────────────────────────────────────────────────────
        private static readonly string[] s_JP = new string[EL.COUNT]
        {
            /* 000 */ "システム参照",
            /* 001 */ "設定",
            /* 002 */ "エフェクト",
            /* 003 */ "ロール",
            /* 004 */ "デバッグ",
            /* 005 */ "参照",
            /* 006 */ "オーディオ",
            /* 007 */ "ユーザーインターフェース",
            /* 008 */ "オプション",
            /* 009 */ "パフォーマンス設定",
            /* 010 */ "刑務所設定",
            /* 011 */ "チェック設定",
            /* 012 */ "ゾーン設定",
            /* 013 */ "位置とスケール",
            /* 014 */ "ビルボード（カメラを向く）",
            /* 015 */ "アニメーション（任意）",
            /* 016 */ "プレイヤー設定",
            /* 017 */ "管理者設定",
            /* 018 */ "グローバル設定",
            /* 019 */ "入力設定",
            /* 020 */ "キャンバス配置",
            /* 021 */ "ユーザーホワイトリスト",
            /* 022 */ "ロールチェック",
            "","","","","","","",
            /* 030 */ "ロールデータベース",
            /* 031 */ "ロールマネージャー",
            /* 032 */ "デフォルトロール",
            /* 033 */ "最大プレイヤー数",
            /* 034 */ "刑務所エリア",
            /* 035 */ "テレポートポイント",
            /* 036 */ "位置チェック間隔（秒）",
            /* 037 */ "動的ロールチェック",
            /* 038 */ "ロールチェック間隔（秒）",
            /* 039 */ "テレポート音",
            /* 040 */ "テレポートパーティクル",
            /* 041 */ "追放ポイント",
            /* 042 */ "追放メッセージ",
            /* 043 */ "追放音",
            /* 044 */ "割り当て音",
            /* 045 */ "割り当てるロール",
            /* 046 */ "ロールなし時のみ",
            /* 047 */ "付与するロール",
            /* 048 */ "認可ユーザー",
            /* 049 */ "未認可ユーザーに非表示",
            /* 050 */ "付与音",
            /* 051 */ "チェック間隔（秒）",
            /* 052 */ "デフォルト表示",
            /* 053 */ "権限あり時に表示",
            /* 054 */ "権限なし時に表示",
            /* 055 */ "頭部上の高さ",
            /* 056 */ "スケール",
            /* 057 */ "ビルボード有効",
            /* 058 */ "Y軸のみ",
            /* 059 */ "アニメーションクリップ",
            /* 060 */ "ループ",
            /* 061 */ "Xフレームごとに更新",
            /* 062 */ "最大数",
            /* 063 */ "設定",
            /* 064 */ "対応するプレファブ",
            /* 065 */ "参加時に割り当て",
            /* 066 */ "割り当て遅延（秒）",
            /* 067 */ "デバッグモード",
            /* 068 */ "ロール割り当て音",
            /* 069 */ "フィードバックテキスト",
            /* 070 */ "プレイヤーごとのMax Slots",
            /* 071 */ "フォールバックプレファブ",
            /* 072 */ "選択キャンバス",
            /* 073 */ "ボタンコンテナ",
            /* 074 */ "メニューを開く",
            /* 075 */ "アクセス拒否",
            /* 076 */ "ロール更新間隔（秒）",
            /* 077 */ "現在のロールテキスト",
            /* 078 */ "最大ボタン数",
            /* 079 */ "トグルキーを有効化",
            /* 080 */ "PCキー",
            /* 081 */ "Questボタン",
            /* 082 */ "プレイヤーの前に配置",
            /* 083 */ "距離（メートル）",
            /* 084 */ "高さ（メートル）",
            /* 085 */ "追加回転（X Y Z）",
            /* 086 */ "キャンバスのスケール（X Y Z）",
            /* 087 */ "開く音",
            /* 088 */ "閉じる音",
            /* 089 */ "エラー音",
            /* 090 */ "閉じる時に入力をクリア",
            /* 091 */ "入力中に情報表示",
            /* 092 */ "情報更新間隔（秒）",
            /* 093 */ "ロール割り当て後に閉じる",
            /* 094 */ "閉じる遅延（秒）",
            /* 095 */ "プレイヤー名入力",
            /* 096 */ "プレイヤー情報テキスト",
            /* 097 */ "キャンバスルート",
            /* 098 */ "距離（× アバター身長）",
            /* 099 */ "垂直オフセット（m）",
            /* 100 */ "基本アバター身長（m）",
            /* 101 */ "基本キャンバススケール",
            /* 102 */ "プレイヤーを向く",
            /* 103 */ "水平面のみ",
            /* 104 */ "クールダウン（秒）",
            /* 105 */ "ディスプレイプレファブ",
            /* 106 */ "チェック間隔（秒）",
            /* 107 */ "割り当て遅延（秒）",
            /* 108 */ "ロールなし（-1）",
            "","","","","","","","","","","",
            /* 120 */ "+ 新しいロールを追加",
            /* 121 */ "+ プレイヤーを追加",
            /* 122 */ "+ 管理者を追加",
            /* 123 */ "+ ユーザーを追加",
            /* 124 */ "+ ロールスロットを追加",
            /* 125 */ "すべて選択",
            /* 126 */ "すべて選択解除",
            /* 127 */ "全ディスプレイを更新",
            /* 128 */ "サイズを同期",
            /* 129 */ "🔧 配列を修復",
            /* 130 */ "iNameGetRoleから自動同期",
            /* 131 */ "↺  iNameGetRoleから同期",
            /* 132 */ "リセット（Y=180）",
            /* 133 */ "回転なし",
            /* 134 */ "インストール",
            /* 135 */ "セットアップを開く",
            /* 136 */ "ドキュメント",
            "","","","","","","","","","","","","",
            /* 150 */ "iRoleDatabaseを割り当ててください",
            /* 151 */ "iPlayerRoleManagerを割り当ててください",
            /* 152 */ "シーンにiRoleDatabaseが見つかりません",
            /* 153 */ "ロールを設定するにはiRoleDatabaseを割り当ててください",
            /* 154 */ "ConfigsとPrefabsの配列は同じサイズである必要があります",
            /* 155 */ "システムは{0}秒ごとにロールの変更を確認します",
            /* 156 */ "ロールチェックが無効です（0 = 無効）",
            /* 157 */ "正確なVRChatのdisplayNamesリスト",
            /* 158 */ "プレイヤーが設定されていません。自動ロール割り当てのためにプレイヤーを追加してください！",
            /* 159 */ "データのあるプレイヤー追加後は変更しないでください！",
            /* 160 */ "⚠️ 内部配列のサイズが正しくありません。「配列を修復」をクリックして修正してください。",
            /* 161 */ "このプレイヤーにはロールが割り当てられていません。ロールスロットを追加してください。",
            /* 162 */ "最大{0}スロットに達しました。「Max Slotsプレイヤーごと」を増やしてください。",
            /* 163 */ "利用可能なロールを表示するには、CoreでiRoleDatabaseを割り当ててください。",
            /* 164 */ "— 空 —",
            /* 165 */ "スロット{0}：（空）",
            /* 166 */ "リストされたビヘイビアは「_OnPlayerRoleChanged」イベントを受け取ります",
            /* 167 */ "各設定はディスプレイの種類を定義します。\nプレファブ配列はconfigs配列と一致する必要があります。",
            /* 168 */ "キャンバストランスフォームなし：メニューは開きますが再配置されません。",
            /* 169 */ "このリストのプレイヤーのみが設定されたキーでメニューを開けます。",
            /* 170 */ "キャンバスはプレイヤーがキーを押すたびに前に配置され、アバターの実際の身長に応じてスケーリングされます。",
            /* 171 */ "トリガーが機能するようにiNameGetRoleを割り当ててください。",
            /* 172 */ "キャンバストランスフォームなし：メニューは開きますが再配置されません。",
            /* 173 */ "ロールが定義されたiRoleDatabaseを割り当ててください",
            /* 174 */ "管理者が設定されていません。「+ 管理者を追加」をクリックしてください。",
            /* 175 */ "管理者が設定済みの状態では変更しないでください。",
            "","","","","","","","","","","","","","",
            /* 190 */ "⚠️ Max Slotsを変更",
            /* 191 */ "設定済みのプレイヤーがある状態で変更すると、すべてのロール設定が削除されます。\n\n続行しますか？",
            /* 192 */ "はい、リセット",
            /* 193 */ "キャンセル",
            /* 194 */ "プレイヤーを削除",
            /* 195 */ "「{0}」とすべてのロール設定を削除しますか？",
            /* 196 */ "削除",
            /* 197 */ "キャンセル",
            /* 198 */ "管理者を削除",
            /* 199 */ "管理者「{0}」とすべてのスロットを削除しますか？",
            /* 200 */ "インストール完了",
            /* 201 */ "{0}個のモジュールをインストールしました：\n\n{1}",
            /* 202 */ "すばらしい！",
            /* 203 */ "変更なし",
            /* 204 */ "モジュールはインストールされませんでした。すでにインストール済みか、プレファブが見つからない可能性があります。",
            /* 205 */ "エラー",
            /* 206 */ "シーンにプレファブ「{0}」が見つかりません。\n\nモジュールをインストールする前に、メインシステムプレファブがシーンにあることを確認してください。",
            /* 207 */ "「{1}」内にオブジェクト「{0}」が見つかりません。\n\nプレファブの構造を確認してください。",
            /* 208 */ "OK",
            /* 209 */ "割り当てられたiNameGetRoleにはプレイヤーが設定されていません。",
            "","","","","","","","","","",
            /* 220 */ "概要：",
            /* 221 */ "• 合計プレイヤー数：{0}",
            /* 222 */ "• 設定されたロールスロット合計：{0}",
            /* 223 */ "• 参加時に割り当て：{0}",
            /* 224 */ "• 遅延：{0}秒",
            /* 225 */ "設定済みの割り当て：",
            /* 226 */ "スケールプレビュー：",
            /* 227 */ "⌨  キーボードキー",
            /* 228 */ "🎮  Oculus / VRボタン",
            /* 229 */ "向き",
            /* 230 */ "スケール",
            /* 231 */ "位置",
            /* 232 */ "定義されたロール",
            /* 233 */ "ロールカラー",
            /* 234 */ "ロールアイコン（任意）",
            /* 235 */ "+ 新しいロールを追加",
            /* 236 */ "新しいロール",
            /* 237 */ "ランタイム情報",
            /* 238 */ "（データはプレイモードのみ表示）",
            /* 239 */ "ロール変更通知",
            /* 240 */ "ユーザーインターフェース",
            /* 241 */ "サウンド",
            /* 242 */ "フィードバックテキスト",
            /* 243 */ "自動クローズ",
            /* 244 */ "コントロールタイプ",
            /* 245 */ "回転とスケール",
            /* 246 */ "Questボタン：",
            /* 247 */ "[ {0} ] を押すとメニューが開きます",
            /* 248 */ "InputName: {0}",
            /* 249 */ "合計：{0}ユーザー",
            /* 250 */ "コントロールタイプ",
            /* 251 */ "モジュールセットアップ",
            /* 252 */ "選択されたモジュール：",
            /* 253 */ "（なし）",
            /* 254 */ "モジュールセットアップ",
            /* 255 */ "インストールするモジュールを以下から選択してください",
            /* 256 */ "モジュール\niRankDisplay",
            /* 257 */ "頭上の\nバッジ",
            /* 258 */ "モジュール\niObjects",
            /* 259 */ "ロール別\nオブジェクト表示",
            /* 260 */ "モジュール\niPrison",
            /* 261 */ "プレイヤー向け\n刑務所",
            /* 262 */ "モジュール\niPrivateRoleZone",
            /* 263 */ "ロール別\nプライベートゾーン",
            /* 264 */ "インストールするモジュールを少なくとも1つ選択してください",
            "","","","",""
        };

        // ── Tabla ZH ──────────────────────────────────────────────────────
        private static readonly string[] s_ZH = new string[EL.COUNT]
        {
            /* 000 */ "系统引用",
            /* 001 */ "配置",
            /* 002 */ "效果",
            /* 003 */ "角色",
            /* 004 */ "调试",
            /* 005 */ "引用",
            /* 006 */ "音频",
            /* 007 */ "用户界面",
            /* 008 */ "选项",
            /* 009 */ "性能设置",
            /* 010 */ "监狱配置",
            /* 011 */ "检查配置",
            /* 012 */ "区域配置",
            /* 013 */ "位置与缩放",
            /* 014 */ "广告牌（面向摄像机）",
            /* 015 */ "动画（可选）",
            /* 016 */ "玩家配置",
            /* 017 */ "管理员配置",
            /* 018 */ "全局配置",
            /* 019 */ "输入配置",
            /* 020 */ "画布定位",
            /* 021 */ "用户白名单",
            /* 022 */ "角色检查",
            "","","","","","","",
            /* 030 */ "角色数据库",
            /* 031 */ "角色管理器",
            /* 032 */ "默认角色",
            /* 033 */ "最大玩家数",
            /* 034 */ "监狱区域",
            /* 035 */ "传送点",
            /* 036 */ "位置检查间隔（秒）",
            /* 037 */ "动态角色检查",
            /* 038 */ "角色间隔（秒）",
            /* 039 */ "传送音效",
            /* 040 */ "传送粒子",
            /* 041 */ "驱逐点",
            /* 042 */ "驱逐消息",
            /* 043 */ "驱逐音效",
            /* 044 */ "分配音效",
            /* 045 */ "要分配的角色",
            /* 046 */ "仅当无角色时",
            /* 047 */ "要授予的角色",
            /* 048 */ "授权用户",
            /* 049 */ "对未授权用户隐藏",
            /* 050 */ "授予音效",
            /* 051 */ "检查间隔（秒）",
            /* 052 */ "默认可见性",
            /* 053 */ "有权限时可见",
            /* 054 */ "无权限时可见",
            /* 055 */ "头部上方高度",
            /* 056 */ "缩放",
            /* 057 */ "启用广告牌",
            /* 058 */ "仅Y轴",
            /* 059 */ "动画片段",
            /* 060 */ "循环",
            /* 061 */ "每X帧更新",
            /* 062 */ "最大容量",
            /* 063 */ "配置",
            /* 064 */ "对应预制体",
            /* 065 */ "加入时分配",
            /* 066 */ "分配延迟（秒）",
            /* 067 */ "调试模式",
            /* 068 */ "角色分配音效",
            /* 069 */ "反馈文本",
            /* 070 */ "每玩家最大Slots",
            /* 071 */ "备用预制体",
            /* 072 */ "选择画布",
            /* 073 */ "按钮容器",
            /* 074 */ "打开菜单",
            /* 075 */ "拒绝访问",
            /* 076 */ "角色更新间隔（秒）",
            /* 077 */ "当前角色文本",
            /* 078 */ "最大按钮数",
            /* 079 */ "启用切换键",
            /* 080 */ "PC按键",
            /* 081 */ "Quest按钮",
            /* 082 */ "在玩家前方定位",
            /* 083 */ "距离（米）",
            /* 084 */ "高度（米）",
            /* 085 */ "额外旋转（X Y Z）",
            /* 086 */ "画布缩放（X Y Z）",
            /* 087 */ "打开音效",
            /* 088 */ "关闭音效",
            /* 089 */ "错误音效",
            /* 090 */ "关闭时清除输入",
            /* 091 */ "输入时显示信息",
            /* 092 */ "信息间隔（秒）",
            /* 093 */ "分配后关闭",
            /* 094 */ "关闭延迟（秒）",
            /* 095 */ "玩家名称输入",
            /* 096 */ "玩家信息文本",
            /* 097 */ "画布根节点",
            /* 098 */ "距离（× 角色身高）",
            /* 099 */ "垂直偏移（m）",
            /* 100 */ "基础角色身高（m）",
            /* 101 */ "基础画布缩放",
            /* 102 */ "面向玩家",
            /* 103 */ "仅水平面",
            /* 104 */ "冷却时间（秒）",
            /* 105 */ "显示预制体",
            /* 106 */ "检查间隔（秒）",
            /* 107 */ "分配延迟（秒）",
            /* 108 */ "无角色（-1）",
            "","","","","","","","","","","",
            /* 120 */ "+ 添加新角色",
            /* 121 */ "+ 添加玩家",
            /* 122 */ "+ 添加管理员",
            /* 123 */ "+ 添加用户",
            /* 124 */ "+ 添加角色槽",
            /* 125 */ "全选",
            /* 126 */ "取消全选",
            /* 127 */ "刷新所有显示",
            /* 128 */ "同步大小",
            /* 129 */ "🔧 修复数组",
            /* 130 */ "从iNameGetRole自动同步",
            /* 131 */ "↺  从iNameGetRole同步",
            /* 132 */ "重置（Y=180）",
            /* 133 */ "无旋转",
            /* 134 */ "安装",
            /* 135 */ "打开设置",
            /* 136 */ "文档",
            "","","","","","","","","","","","","",
            /* 150 */ "请分配iRoleDatabase",
            /* 151 */ "请分配iPlayerRoleManager",
            /* 152 */ "场景中未找到iRoleDatabase",
            /* 153 */ "请分配iRoleDatabase以配置角色",
            /* 154 */ "Configs和Prefabs数组必须大小相同",
            /* 155 */ "系统将每{0}秒检查一次角色变化",
            /* 156 */ "角色检查已禁用（0 = 禁用）",
            /* 157 */ "精确的VRChat displayNames列表",
            /* 158 */ "没有配置玩家。添加玩家以自动分配角色！",
            /* 159 */ "添加有数据的玩家后请勿更改！",
            /* 160 */ "⚠️ 内部数组大小不正确。点击「修复数组」进行修复。",
            /* 161 */ "此玩家没有分配角色。请添加角色槽。",
            /* 162 */ "已达到最大{0}个槽。请增加「每玩家最大Slots」。",
            /* 163 */ "请在Core中分配iRoleDatabase以查看可用角色。",
            /* 164 */ "— 空 —",
            /* 165 */ "槽 {0}：（空）",
            /* 166 */ "列出的行为将接收「_OnPlayerRoleChanged」事件",
            /* 167 */ "每个配置定义一种显示类型。\n预制体数组必须与configs数组匹配。",
            /* 168 */ "无画布Transform：菜单将打开但不会重新定位。",
            /* 169 */ "只有此列表中的玩家才能使用配置的按键打开菜单。",
            /* 170 */ "每次玩家按键时，画布将定位在玩家前方，并根据角色的实际身高缩放。",
            /* 171 */ "请分配iNameGetRole以使触发器工作。",
            /* 172 */ "无画布Transform：菜单将打开但不会重新定位。",
            /* 173 */ "请分配已定义角色的iRoleDatabase",
            /* 174 */ "没有配置管理员。点击「+ 添加管理员」开始。",
            /* 175 */ "已配置管理员后请勿更改。",
            "","","","","","","","","","","","","","",
            /* 190 */ "⚠️ 更改最大槽数",
            /* 191 */ "已配置玩家的情况下更改将删除所有角色配置。\n\n继续？",
            /* 192 */ "是，重置",
            /* 193 */ "取消",
            /* 194 */ "删除玩家",
            /* 195 */ "删除「{0}」及其所有角色配置？",
            /* 196 */ "删除",
            /* 197 */ "取消",
            /* 198 */ "删除管理员",
            /* 199 */ "删除管理员「{0}」及其所有槽？",
            /* 200 */ "安装完成",
            /* 201 */ "已安装{0}个模块：\n\n{1}",
            /* 202 */ "太棒了！",
            /* 203 */ "无变化",
            /* 204 */ "没有安装任何模块。部分可能已安装或未找到预制体。",
            /* 205 */ "错误",
            /* 206 */ "场景中未找到预制体「{0}」。\n\n请确保在安装模块之前场景中有系统主预制体。",
            /* 207 */ "在「{1}」中未找到对象「{0}」。\n\n请检查预制体结构。",
            /* 208 */ "确定",
            /* 209 */ "分配的iNameGetRole没有配置玩家。",
            "","","","","","","","","","",
            /* 220 */ "摘要：",
            /* 221 */ "• 玩家总数：{0}",
            /* 222 */ "• 已配置角色槽总数：{0}",
            /* 223 */ "• 加入时分配：{0}",
            /* 224 */ "• 延迟：{0}秒",
            /* 225 */ "已配置的分配：",
            /* 226 */ "缩放预览：",
            /* 227 */ "⌨  键盘按键",
            /* 228 */ "🎮  Oculus / VR按钮",
            /* 229 */ "方向",
            /* 230 */ "缩放",
            /* 231 */ "位置",
            /* 232 */ "已定义角色",
            /* 233 */ "角色颜色",
            /* 234 */ "角色图标（可选）",
            /* 235 */ "+ 添加新角色",
            /* 236 */ "新角色",
            /* 237 */ "运行时信息",
            /* 238 */ "（数据仅在播放模式下可见）",
            /* 239 */ "角色变更通知",
            /* 240 */ "用户界面",
            /* 241 */ "音效",
            /* 242 */ "反馈文本",
            /* 243 */ "自动关闭",
            /* 244 */ "控制类型",
            /* 245 */ "旋转与缩放",
            /* 246 */ "Quest按钮：",
            /* 247 */ "按 [ {0} ] 打开菜单",
            /* 248 */ "InputName: {0}",
            /* 249 */ "总计：{0}位用户",
            /* 250 */ "控制类型",
            /* 251 */ "模块设置",
            /* 252 */ "已选模块：",
            /* 253 */ "（无）",
            /* 254 */ "模块设置",
            /* 255 */ "请在下方选择要安装的模块",
            /* 256 */ "模块\niRankDisplay",
            /* 257 */ "头顶\n徽章",
            /* 258 */ "模块\niObjects",
            /* 259 */ "按角色\n对象可见性",
            /* 260 */ "模块\niPrison",
            /* 261 */ "玩家\n监狱",
            /* 262 */ "模块\niPrivateRoleZone",
            /* 263 */ "按角色\n私人区域",
            /* 264 */ "请至少选择一个模块进行安装",
            "","","","",""
        };

        // =========================================================================
        // API PÚBLICA
        // =========================================================================

        // ── Caché de idioma ────────────────────────────────────────────────
        // FIX LAG: GetLang() llama a FindObjectOfType cada vez que se dibuja
        // un string en el inspector (decenas de veces por frame). Cacheamos.
        private static int  _cachedLang         = -1;
        private static bool _langCacheValid      = false;

        /// <summary>Invalida la caché de idioma (útil al cambiar de escena).</summary>
        public static void InvalidateLangCache() { _langCacheValid = false; }

        /// <summary>Obtiene el idioma activo leyendo el iRoleSystemCore en la escena (cacheado).</summary>
        public static int GetLang()
        {
            if (_langCacheValid) return _cachedLang;
            Core.iRoleSystemCore core = Object.FindObjectOfType<Core.iRoleSystemCore>();
            _cachedLang      = (core == null) ? 0 : (int)core.systemLanguage;
            _langCacheValid  = true;
            return _cachedLang;
        }

        /// <summary>Obtiene string localizado por ID.</summary>
        public static string Get(int id)
        {
            return Get(id, GetLang());
        }

        public static string Get(int id, int lang)
        {
            string[] table = GetTable(lang);
            if (id < 0 || id >= table.Length) return "[?]";
            string s = table[id];
            return string.IsNullOrEmpty(s) ? "[?]" : s;
        }

        /// <summary>Obtiene string con un argumento {0}.</summary>
        public static string Get(int id, string arg0)
        {
            return Get(id).Replace("{0}", arg0);
        }

        public static string Get(int id, string arg0, string arg1)
        {
            return Get(id).Replace("{0}", arg0).Replace("{1}", arg1);
        }

        private static string[] GetTable(int lang)
        {
            switch (lang)
            {
                case 0: return s_ES;
                case 1: return s_EN;
                case 2: return s_JP;
                case 3: return s_ZH;
                default: return s_ES;
            }
        }
    }
}
#endif
