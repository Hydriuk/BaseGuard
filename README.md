<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 21cm 150cm; }
        body { margin: 0.2cm; }
    }
</style>

# **BaseGuard** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub>

This plugin allows to protect player's structures and barricades when in range of specific barricades.  
This protection can either be always active, or only be active when all group members are disconnected.

1. [**Configuration examples**](#configuration-examples)
   1. [OpenMod](#openmod)
   2. [RocketMod](#rocketmod)
2. [**Options description**](#options-description)
   1. [**Protection controls**](#protection-controls)
      1. [ActivationMode](#activationmode)
      2. [BaseShield](#baseshield)
      1. [ProtectedGroups](#protectedgroups)
      2. [Schedule](#schedule)
      3. [Overrides](#overrides)
      4. [AllowSelfDamage](#allowselfdamage)
   2. [**Guards**](#guards)
      1. [GuardMode](#guardmode)
      2. [Guards](#guards-1)
   3. [**Chat messages**](#chat-messages)
      1. [DamageWarnCooldown](#damagewarncooldown)
      2. [ChatIcon](#chaticon)
   4. [**Anti-abuse rules**](#anti-abuse-rules)
      1. [GroupHistoryDuration](#grouphistoryduration)
      2. [RaidDuration](#raidduration)
      3. [ProtectionDuration](#protectionduration)

## **Configuration examples**
### **OpenMod**
```yaml
ActivationMode: Offline
BaseShield: 0
ProtectedGroups: All
Schedule:
- Protection: On
  At: 0 0 * * 1-5
- Protection: Off
  At: 0 18 * * 1-5
- Protection: On
  At: 0 2 * * 6,0
- Protection: Off
  At: 0 10 * * 6,0
Overrides:
- Id: 1373
  BaseShield: 0
  MaxShield: 0
AllowSelfDamage: false

GuardMode: Ratio
Guards:
- Id: 458
  Range:  16
  Shield: 0.5
- Id: 1230
  Range: 64
  Shield: 1

ChatMessages: 
  DamageWarnCooldown: 10
  ChatIcon: https://i.imgur.com/V6Jc0S7.png
  EffectID: 0
  EffectTextName: Message
  EffectDuration: 5

GroupHistoryDuration: 48
RaidDuration: 120
ProtectionDuration: 24
```

### **RocketMod**
```xml
<?xml version="1.0" encoding="utf-8"?>
<RocketConfiguration 
  xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
>
  <ActivationMode>Offline</ActivationMode>
  <BaseShield>0.5</BaseShield>
  <ProtectedGroups>Any</ProtectedGroups>
  <Schedule>
    <ScheduledProtection Protection="On" At="0 0 * * 1-5" />
    <ScheduledProtection Protection="Off" At="0 18 * * 1-5" />
    <ScheduledProtection Protection="On" At="0 2 * * 6,0" />
    <ScheduledProtection Protection="Off" At="0 10 * * 6,0" />
  </Schedule>
 <Overrides>
    <ShieldOverride>
      <Id>1373</Id>
      <BaseShield>0</BaseShield>
      <MaxShield>0</MaxShield>
    </ShieldOverride>
  </Overrides>
  <AllowSelfDamage>false</AllowSelfDamage>
  <GuardMode>Base</GuardMode>
  <Guards>
    <GuardAsset>
      <Id>458</Id>
      <Range>16</Range>
      <Shield>0.5</Shield>
    </GuardAsset>
    <GuardAsset>
      <Id>1230</Id>
      <Range>64</Range>
      <Shield>1</Shield>
    </GuardAsset>
  </Guards>
  <ChatMessages 
    Cooldown="10" 
    ChatIcon="https://i.imgur.com/V6Jc0S7.png" 
    EffectID="0" 
    EffectTextName="Message" 
    EffectDuration="5">
  </ChatMessages>
  <GroupHistoryDuration>48</GroupHistoryDuration>
  <RaidDuration>120</RaidDuration>
  <ProtectionDuration>24</ProtectionDuration>
</RocketConfiguration>
```

## **Options description**

---

### **Protection controls**
#### **ActivationMode**
Controls general protection activation. Values :
- `Unabled` : Protections are never applied
- `Offline` : Protections are active when the player who placed the structure and his group  are disconnected
- `Permanent` : Protections are always applied

#### **BaseShield**
Base protection to be applied when structures and barricades are protected.  
The value is the damage multiplier for protected strutures.  
Examples :  
`0.5` will virtually increase the structure's life by 2x.
`0.66` will virtually increase the structure's life by 1.5x.
The calculus is 1 / 0.66 = 1.5

#### **ProtectedGroups**
Controls protection activation depending on the group's type. Values : 
- `NoGroup` : Only structures that are not part of a group are protected
- `InGameGroup` : Only structures which group is an in-game group are protected
- `SteamGroup` : Only structures which group is a steam group are protected
- `Any` : Structures are protected, independently of their group type

#### **Schedule**
Controls when are protection active for everybody. With this you can for example enable  protection during the day and disable it on weekends.
- `Protection`: Values : `On` / `Off`. Turn the protection on or off
- `At`: Value : A cron table. Here is a link to help with the syntax : https://crontab.guru/. The  protection will change state at the moment defined by the cron table.

Schedule example : 
```yaml
Schedule:
- Protection: On
  At: 0 0 * * 1-5
- Protection: Off
  At: 0 18 * * 1-5
- Protection: On
  At: 0 2 * * 6,0
- Protection: Off
  At: 0 10 * * 6,0
```
This schedule will enable protections from 00:00 to 18:00 during the week, and from 02:00 to 10:00 on weekends. Bases will be raidable from 18:00 to 00:00, monday to friday. And from 10:00 to 02:00 saturday and  sunday.  
If not clear enough, you may contact me for help.

#### **Overrides**
Changes the protection for specific barricades and structures
- `Id` : Id of the barricade/structure being overrided
- `BaseShield` : New base shield for this structure
- `MaxShield` : Maximum shield the structure can have

The overrides allows you to set different behaviours for given structures and barricades. For example, you can set `BaseShield` and `MaxShield` of sentries to 0 to prevent them from having any protection.

#### **AllowSelfDamage**
Allow players to deal normal damage to their own structures. *Values* : `true` or `false`

---

### **Guards**
#### **GuardMode**
Controls protection calculation. Values :
- `Base` : Only the value from `BaseShield` is used. `Guards` are ignored.
- `Cumulative` : `BaseShield` and `Shield`'s value from in range `Guards` are added. Example : `BaseShield: 0.5` and a guard with `Shield: 0.5` will give a total protection of `100%` : (`0.5` + `0.5`) * 100
- `Ratio` : `BaseShield` and `Shield`'s value from in range `Guards` are multiplied. Example : `BaseShield: 0.5` and a guard with `Shield: 0.5` will give a total protection of `75%` : (`1` - (`1` -`0.5`) * (`1` - `0.5`)) * 100  
For each structures, only one of each type of guards in taken into account : If you have two small generators in range, one of them will be ignored.

#### **Guards**
Barricade and structure guards.  
- `Id` : Id of the guard
- `Range` : Range of the guard's protection
- `Shield` : Amount of protection the guard gives

A guard can either be a barricade or a structure. A guard will protect in range buildables by a certain amount.
A generator guard must be powered to provide protection, as well as safezone radiators and oxygenators.
When calculating protection for a buildable, only one guard of each type will be used : two small generators won't add their shields, but a small and a large one will.

---

### **Chat Messages**
#### **Cooldown**
Value is in seconds. When a player tries to damage a protected structure, he will receive a warning. To prevent chat spam, this value tells the plugin to wait between messages.

#### **ChatIcon**
Icon used when the plugin sends a message to a player

#### **EffectID**
ID of the effect to display in place of the chat message. If this id is set, the chat message won't show.

#### **EffectTextName**
Name of the effect's gameobject in which the message should be written

#### **EffectDuration**
How long should the effect stay on screen

---

### **Anti-abuse rules**
#### **GroupHistoryDuration**
Value is in hours. How long a player is still considered part of a group by the plugin after quitting it. 
*Example*: `GroupHistoryDuration: 48` : When the player is connected, all groups he has been in for the last 48 hours will not be protected.

#### **RaidDuration**
Value is in seconds. When a player being raided disconnects, the raiders can continue to raid the base as long as they deal a damage every X seconds.  
If the nobody deals damage to the structures of the disconnected player for the set amount of time, then the protection apply.  
Raiders cannot start a raid if the owner disconnected before they dealt damage to the base.

#### **ProtectionDuration**
Value is in hours. How long does offline protection lasts
*Example* : `ProtectionDuration: 24` : 24 hours after the protection was applied to the group's structures, the protection will wear off.

---