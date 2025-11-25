# รีวิวโค้ด Quest System

- โค้ดระบบ Quest วางโครงสร้างเป็นโมดูลย่อยๆ (ScriptableObject สำหรับข้อมูลเควส, QuestManager สำหรับสถานะเควส, UI แยกเป็นส่วน) ซึ่งเป็นแนวทางที่ดี — แยก data ออกจาก Behaviour ทำให้ designer สามารถสร้างเควสใน Resources ได้ อันนี้ดีครับ

ข้อดีหลัก

- ใช้ `ScriptableObject` (`Assets/Scripts/QuestSystem/QuestData.cs`) สำหรับเก็บข้อมูลเควส — ดีสำหรับการจัดการข้อมูลที่ไม่ขึ้นกับ instance
- มีการแยก UI (`QuestUIManager`, `QuestSlotUI`) ออกจาก logic (QuestManager) ทำให้หน้าที่ของแต่ละชั้นชัดเจน
- ระบบ QuestObjective รองรับ quest หลายประเภท objective (Kill/Collect/Talk) ใน `QuestObjective` และเก็บค่า `requiredAmount` ทำให้ขยายเพื่อพัฒนาต่อยอดได้ง่าย

ประเด็นที่ควรปรับปรุง (เรียงตามความสำคัญ)

1. บั๊กร้ายแรงใน `ResetProgress()`

- file: `Assets/Scripts/QuestSystem/QuestManager.cs`
- feedback: เงื่อนไขในเมธอด `ResetProgress()` เป็นดังนี้:

```csharp
public void ResetProgress()
{
    if (currentQuest == null && currentQuest.questData != null && !currentQuest.isCompleted)
    {
        currentQuest.currentProgress = 0;
    }
}
```

เงื่อนไข `currentQuest == null && currentQuest.questData != null` จะทำให้เกิด NullReference หรือเงื่อนไขไม่เคยเป็นจริง — สิ่งที่น่าจะต้องการคือเช็ค `currentQuest != null` แทน ไม่น่าจะใช่ `currentQuest == null`

2. การใช้ Singleton / การจัดการ instance

- file: `QuestUIManager.cs`
- feedback: `DontDestroyOnLoad` ถูกเรียกที่ `QuestManager` แต่ `QuestUIManager` ไม่มี — ทำให้ความคาดหวังต่างกันเมื่อเปลี่ยน scene

3. ซ้ำซ้อนและตรวจสอบเงื่อนไขไม่จำเป็น

- ไฟล์: `QuestManager.AcceptQuest` มี `HasActiveQuest()` ถูกเรียกตรวจ 2 ครั้ง!!!!

4. ระบบ TimeTrial ยังไม่มีการจัดการเวลาที่ชัดเจนใน QuestManager/QuestProgress

- `QuestData` มี `questTimeLimit` และ `QuestType.TimeTrail` แต่ไม่เห็นการ decrement หรือตรวจจับเวลาหมดใน `QuestManager` หรือ `QuestProgress` ไม่แน่จว่าเป็นตัวแปรที่ตั้งใจจะใช้หรือไม่ครับ

5. Persistence (บันทึกสถานะเควส) --- สำหรับปรับปรุงในอนาคตครับ

- ปัจจุบันไม่มีระบบ Save/Load สำหรับ `currentQuest` หรือ `completedQuests` — ถ้าต้องการให้สถานะอยู่ต่อเมื่อปิดเกม ควรเพิ่ม serialization (เช่น JSON, PlayerPrefs หรือระบบไฟล์) และแปลง `QuestData` reference เป็น ID เพื่อบันทึก/เรียกคืน
