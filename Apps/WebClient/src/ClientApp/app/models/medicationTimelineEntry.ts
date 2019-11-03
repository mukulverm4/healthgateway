import TimelineEntry, { EntryType } from "@/models/timelineEntry";
import MedicationStatement from "@/models/medicationStatement";
import Pharmacy from "./pharmacy";
import MedicationSumary from "./medicationSumary";
import MedicationResult from "./medicationResult";

// The medication timeline entry model
export default class MedicationTimelineEntry extends TimelineEntry {
  public pharmacy: PharmacyViewModel;
  public medication: MedicationViewModel;

  public practitionerSurname: string;
  public prescriptionIdentifier: string;

  public constructor(model: MedicationStatement) {
    super("id-" + Math.random(), model.dispensedDate, EntryType.Medication);
    this.pharmacy = new PharmacyViewModel(model.pharmacyId);
    this.medication = new MedicationViewModel(model.medicationSumary);

    this.practitionerSurname = model.practitionerSurname
      ? model.practitionerSurname
      : "N/A";
    this.prescriptionIdentifier = model.prescriptionIdentifier
      ? model.prescriptionIdentifier
      : "N/A";
  }
}

class PharmacyViewModel {
  public id: string;
  public name?: string;
  public address?: string;
  public phoneNumber?: string;

  constructor(id: string | undefined) {
    this.id = id ? id : "";
  }

  public populateFromModel(model: Pharmacy): void {
    this.name = model.name;
    this.phoneNumber = model.phoneNumber;

    this.address =
      model.addressLine1 +
      " " +
      model.addressLine2 +
      ", " +
      model.city +
      " " +
      model.province;
  }
}

class MedicationViewModel {
  public din: string;
  public brandName?: string;
  public genericName?: string;
  public quantity?: number;
  public form?: string;
  public strength?: string;
  public strengthUnit?: string;
  public manufacturer?: string;
  constructor(model: MedicationSumary) {
    this.din = model.din ? model.din : "";
    this.brandName = model.brandName;
    this.genericName = model.genericName;
    this.quantity = model.quantity;
  }

  public populateFromModel(model: MedicationResult): void {
    //console.log(model);
    if (model.federalData) {
      let federalModel = model.federalData;

      this.form = federalModel.forms[0].pharmaceuticalForm
        ? federalModel.forms[0].pharmaceuticalForm
        : "";

      this.strength = federalModel.activeIngredients[0].strength
        ? federalModel.activeIngredients[0].strength
        : "";

      this.strengthUnit = federalModel.activeIngredients[0].strengthUnit
        ? federalModel.activeIngredients[0].strengthUnit
        : "";

      this.manufacturer = federalModel.companies[0].companyName
        ? federalModel.companies[0].companyName
        : "";
    } else if (model.provincialData) {
      let provincialModel = model.provincialData;
      this.form = provincialModel.pharmaCareDrug
        ? provincialModel.pharmaCareDrug.dosageForm
        : "";
    }
  }
}